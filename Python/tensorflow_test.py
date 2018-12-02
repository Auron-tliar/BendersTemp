import tensorflow as tf
from gym_unity.envs import UnityEnv
import numpy as np


def build_model(num_inputs, num_outputs):
    input = tf.placeholder(shape=[None, num_inputs], dtype=tf.float32)
    dense1 = tf.layers.dense(input, 32, activation=tf.nn.relu)
    output = tf.layers.dense(dense1, num_outputs, activation=None)

    return input, output


def main():
    env = UnityEnv("../Build/BendersTemp.exe", 0, use_visual=False)

    observation_size = len(env.observation_space.low)
    action_size = len(env.action_space.low)

    input, output = build_model(observation_size, action_size)

    excpected_value = tf.placeholder(shape=[None, action_size], dtype=tf.float32)

    cost = tf.reduce_mean((output - excpected_value)**2)
    optimizer = tf.train.RMSPropOptimizer(0.01).minimize(cost)

    init = tf.global_variables_initializer()

    with tf.Session() as sess:
        sess.run(init)

        observation = env.reset()
        reward_vec = np.zeros(action_size)

        for step in range(1000):
            _, val, action_vec = sess.run([optimizer, cost, output], feed_dict={input: observation.reshape(-1, observation_size), excpected_value: reward_vec.reshape(-1, action_size)})

            observation, reward, done, info = env.step(action_vec)

            reward_vec = np.zeros(action_size)

            sel_action = np.argmax(action_vec)

            reward_vec[sel_action] = reward

            if step % 10 == 0:
                print("step: {}, value: {} reward: {} sel_action: {}".format(step, val, reward, sel_action))


if __name__ == '__main__':
    main()

