import os
import tensorflow as tf
from gym_unity.envs import UnityEnv
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from tensorflow.python.tools import freeze_graph
import pickle

from DQNAgent import DQNAgent

result_dir = 'results'

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


def plot_observations(observations, observation_header_size):
    # np.set_printoptions(threshold=np.inf)

    for observation in observations:
        w = int(observation[1])
        h = int(observation[2])
        grid = observation[observation_header_size:].reshape(w, h)
        # print(np.sum(grid))
        points_x = []
        points_y = []
        # print(grid)

        for y in range(h):
            for x in range(w):
                if grid[x, y] > 0:
                    points_x.append(x-w/2)
                    points_y.append(y-h/2)

        plt.scatter(points_x, points_y)
        # print(points_x)
        # print(points_y)

        points_x = []
        points_y = []
        # print(grid)

        for y in range(h):
            for x in range(w):
                if grid[x, y] < 0:
                    points_x.append(x-w/2)
                    points_y.append(y-h/2)

        plt.scatter(points_x, points_y)

        plt.xlim((-w/2, w/2))
        plt.ylim((-h/2, h/2))

        plt.show()
        plt.clf()



def main():
    env = UnityEnv("../Build/BendersTemp.exe", 0, use_visual=False, multiagent=True, no_graphics=False)

    observation_header_size = 4
    observation_size = len(env.observation_space.low)
    action_size = len(env.action_space.low)

    minibatch_size = 128
    total_step_count = 100000

    agent = DQNAgent('bender_agent', observation_size, action_size, minibatch_size)

    model_input = tf.placeholder(shape=[None, observation_size], dtype=tf.float32, name="vector_observation")

    model_output = tf.identity(agent.predict(model_input), name="action")

    saver = tf.train.Saver()

    with tf.Session() as sess:

        observations = env.reset()

        #plot_observations(observations)

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

        if os.path.exists('model/weights.index'):
            saver.restore(sess, 'model/weights')
            print('restored model')

            sess.run(agent.reset_epsilon(0.7))

        loss_list = []
        rewards_list = []

        saved_loss_list_data = [f for f in os.listdir(result_dir) if '.dump' in f]
        continue_training = len(saved_loss_list_data) > 0

        if continue_training:
            max_saved = max([(int(d.split('_')[-1].split('.')[0]), d) for d in saved_loss_list_data])
            print(max_saved)
            loss_list = pickle.load(open(result_dir+'/'+max_saved[1], 'rb'))
            min_step = max_saved[0]+1
        else:
            min_step = 0

        for step in range(min_step, total_step_count):
            reshaped_observation = np.array(observations).reshape(-1, observation_size)
            # print('reshaped_observation shape: '+str(reshaped_observation.shape))

            action_vecs = sess.run(agents_act_commands, feed_dict={model_input: reshaped_observation})
            # action_vecs = np.zeros((num_agents, action_size))
            # action_vecs[:, 6] = 1

            action_vecs = list(action_vecs)
            # print('action_vecs: '+str(action_vecs))

            new_observations, rewards, done, info = env.step(action_vecs)

            rewards_list.append(rewards)

            for agent_idx in range(num_agents):
                sel_action = np.argmax(action_vecs[agent_idx])
                history.append(np.concatenate((observations[agent_idx], new_observations[agent_idx], [sel_action], [rewards[agent_idx]])))

            observations = new_observations

            defeated_count = sum([o[3] for o in new_observations])

            if step % 3 == 0:
                if len(history) >= minibatch_size:
                    random_minibatch = np.array(history)[np.random.choice(len(history), size=minibatch_size, replace=False), :]
                    _, model_loss, epsilon = sess.run(history_replay_action, feed_dict={minibatch: random_minibatch})

                    print("step: {}, model_loss: {} reward: {} sel_action: {} epsilon: {}".format(step, model_loss, np.mean(rewards), sel_action, epsilon))

                    loss_list.append(model_loss)

                else:
                    print("step: {}, reward: {} sel_action: {}".format(step, np.mean(rewards), sel_action))

                # reduce histroy size
                if len(history) > 3000:
                    history = history[1500:]

            if step % 400 == 0 and step > 0:
                try:
                    plot_observations(new_observations, observation_header_size)

                    # loss
                    if len(loss_list) > 1000:
                        plt.plot(pd.DataFrame(loss_list[100:]).rolling(window=5).mean())
                    else:
                        plt.plot(pd.DataFrame(loss_list).rolling(window=5).mean())
                    plt.title('loss step: %d' % (step))
                    plt.ylabel('loss')
                    plt.xlabel('step')
                    plt.savefig(result_dir+'/loss_step_%s.pdf' % (step))
                    plt.show()
                    plt.clf()

                    np_rewards = np.array(rewards_list)
                    for i in range(np_rewards.shape[1]):
                        plt.plot(np_rewards[:, i])
                    plt.title('rewards step: %d' % (step))
                    plt.ylabel('reward')
                    plt.xlabel('step')
                    plt.savefig(result_dir+'/rewards_step_%s.pdf' % (step))
                    plt.show()
                    plt.clf()
                except:
                    print('could save plot')

            if (step % 200 == 0 and step) > 0 or defeated_count >= 1:
                observations = env.reset()
                print('resetting environment')

            if step % 1000 == 0 and step > 0:
                saver.save(sess, 'model/weights')
                export_graph(sess)

                pickle.dump(loss_list, open(result_dir+'/loss_step_%s.dump' % step, 'wb'))

        saver.save(sess, 'model/weights')

        export_graph(sess)


if __name__ == '__main__':
    if not os.path.exists('results'):
        os.mkdir('results')

    main()
