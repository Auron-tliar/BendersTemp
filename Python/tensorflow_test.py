import tensorflow as tf
from gym_unity.envs import UnityEnv
import numpy as np
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


def build_model(num_inputs, num_outputs):
    input = tf.placeholder(shape=[None, num_inputs], dtype=tf.float32, name="vector_observation")
    dense1 = tf.layers.dense(input, 32, activation=tf.nn.relu)
    output = tf.layers.dense(dense1, num_outputs, activation=None, name="output")

    output = tf.identity(output, name="action")

    return input, output


def main():
    env = UnityEnv("../Build/BendersTemp.exe", 0, use_visual=False)

    observation_size = len(env.observation_space.low)
    action_size = len(env.action_space.low)

    minibatch_size = 32

    # agent = DQNAgent('a', observation_size, action_size, minibatch_size)

    input, output = build_model(observation_size, action_size)

    excpected_value = tf.placeholder(shape=[None, action_size], dtype=tf.float32)

    loss = tf.losses.mean_squared_error(output, excpected_value)

    optimizer = tf.train.RMSPropOptimizer(0.01).minimize(loss)

    saver = tf.train.Saver()

    init = tf.global_variables_initializer()

    with tf.Session() as sess:
        sess.run(init)

        observation = env.reset()
        reward_vec = np.zeros(action_size)

        for step in range(1000):
            _, val, action_vec = sess.run([optimizer, loss, output],
                                          feed_dict={input: observation.reshape(-1, observation_size),
                                                     excpected_value: reward_vec.reshape(-1, action_size)})

            observation, reward, done, info = env.step(action_vec)

            reward_vec = np.zeros(action_size)

            sel_action = np.argmax(action_vec)

            reward_vec[sel_action] = reward

            if step % 10 == 0:
                print("step: {}, value: {} reward: {} sel_action: {}".format(step, val, reward, sel_action))

        saver.save(sess, 'model/weights')

        print(output)

        export_graph(sess)


if __name__ == '__main__':
    main()
