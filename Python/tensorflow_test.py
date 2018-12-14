import os
import tensorflow as tf
from gym_unity.envs import UnityEnv
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from tensorflow.python.tools import freeze_graph

from DQNAgent import DQNAgent


def export_graph(sess):
    model_path = 'model'

    tf.train.write_graph(sess.graph_def, model_path,  "raw_graph_def.pb", as_text=False)

    freeze_graph.freeze_graph(input_graph=model_path + '/raw_graph_def.pb',
                              input_binary=True,
                              input_checkpoint=tf.train.latest_checkpoint(model_path),
                              output_node_names="action",
                              output_graph=model_path + '/tf_benders.bytes',
                              clear_devices=True, initializer_nodes="", input_saver="",
                              restore_op_name="save/restore_all", filename_tensor_name="save/Const:0")


def main():
    env = UnityEnv("../Build/BendersTemp.exe", 0, use_visual=False, multiagent=True)

    observation_header_size = 3
    observation_size = len(env.observation_space.low)
    action_size = len(env.action_space.low)

    minibatch_size = 32
    total_step_count = 3000

    agent = DQNAgent('bender_agent', observation_size, action_size, minibatch_size)

    model_input = tf.placeholder(shape=[None, observation_size], dtype=tf.float32, name="vector_observation")

    model_output = tf.identity(agent.predict(model_input), name="action")

    saver = tf.train.Saver()

    with tf.Session() as sess:

        observations = env.reset()

        num_agents = len(observations)

        print('num_agents: '+str(num_agents))

        agents_act_commands = [agent.act(model_input[agent_idx]) for agent_idx in range(num_agents)]

        minibatch = tf.placeholder(shape=[minibatch_size, observation_size*2 + 2], dtype=tf.float32, name="history")
        history_replay_action = list(agent.replay(minibatch))
        history_replay_action.append(agent.update_epsilon())

        history = []  # (-1, observation_size*2 + 2)

        sess.run(tf.global_variables_initializer())

        # run dummy model to make sure model_output is used in the graph
        sess.run(model_output, feed_dict={model_input: np.zeros((1, observation_size))})

        loss_list = []

        for step in range(total_step_count):
            reshaped_observation = np.array(observations).reshape(-1, observation_size)
            # print('reshaped_observation shape: '+str(reshaped_observation.shape))

            action_vecs = sess.run(agents_act_commands, feed_dict={model_input: reshaped_observation})

            action_vecs = list(action_vecs)
            # print('action_vecs: '+str(action_vecs))

            new_observations, rewards, done, info = env.step(action_vecs)

            for agent_idx in range(num_agents):
                sel_action = np.argmax(action_vecs[agent_idx])
                history.append(np.concatenate((observations[agent_idx], new_observations[agent_idx], [sel_action], [rewards[agent_idx]])))

            if step % 10 == 0:
                if len(history) >= minibatch_size:
                    random_minibatch = np.array(history)[np.random.choice(len(history), size=minibatch_size, replace=False), :]
                    _, model_loss, epsilon = sess.run(history_replay_action, feed_dict={minibatch: random_minibatch})

                    print("step: {}, model_loss: {} reward: {} sel_action: {} epsilon: {}".format(step, model_loss, np.mean(rewards), sel_action, epsilon))

                    loss_list.append(model_loss)

                else:
                    print("step: {}, reward: {} sel_action: {}".format(step, np.mean(rewards), sel_action))

            if step % 200 == 0 and step > 0:
                try:
                    # loss
                    plt.plot(pd.DataFrame(loss_list).rolling(window=5).mean())
                    plt.title('loss step: %d' % (step))
                    plt.ylabel('loss')
                    plt.xlabel('step')
                    plt.savefig('results/loss_step_%s.pdf' % (step))
                    plt.clf()
                except:
                    print('could save plot')

            if step % 300 == 0 and step > 0:
                observations = env.reset()
                #history = []

        saver.save(sess, 'model/weights')

        export_graph(sess)


if __name__ == '__main__':
    if not os.path.exists('results'):
        os.mkdir('results')

    main()
