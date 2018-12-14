import tensorflow as tf
import numpy as np


def create_network(input_vec, num_inputs, num_outputs, name):
    dense1 = tf.layers.dense(input_vec, 1024, activation=tf.nn.relu, name="dense1", reuse=tf.AUTO_REUSE)
    dense2 = tf.layers.dense(dense1, 256, activation=tf.nn.relu, name="dense2", reuse=tf.AUTO_REUSE)
    output = tf.layers.dense(dense2, num_outputs, activation=None, name="output_layer", reuse=tf.AUTO_REUSE)

    #output = tf.identity(output, name="action")

    return output


# Deep Q-learning Agent
class DQNAgent:
    def __init__(self, agent_name, observation_size, action_size, batch_size):
        self.agent_name = agent_name

        self.observation_size = observation_size
        self.action_size = action_size
        self.gamma = 0.9  # discount rate

        self.epsilon_min = 0.01
        self.epsilon_decay = 0.995
        self.learning_rate = 0.01
        self.batch_size = batch_size

        self.qnet = lambda input_vec: create_network(input_vec, self.observation_size, self.action_size, 'qnet_' + agent_name)

        # self.qnet_optimizer = tf.train.RMSPropOptimizer(learning_rate=self.learning_rate, decay=0.99, epsilon=0.01)
        self.qnet_optimizer = tf.train.AdamOptimizer(learning_rate=self.learning_rate)

        self.epsilon = tf.Variable(initial_value=tf.constant(0.7), dtype=tf.float32, trainable=False)  # exploration rate

    def define_model(self, qnet, input_vec, input_shape, reference=None, trainable=False, return_embedding=False):
        print('defining model')

        input_vec = tf.reshape(tf.stop_gradient(tf.clip_by_value(input_vec, -1000.0, 1000.0)), (-1, input_shape))

        if return_embedding:
            return qnet(input_vec, return_embedding=True)
        else:
            model = qnet(input_vec)

        if trainable:
            with tf.name_scope('model_loss'):

                # ERROR CLIPPING
                error = tf.abs(model - reference)
                quadratic_part = tf.clip_by_value(error, 0.0, 1.0)
                linear_part = error - quadratic_part
                loss = tf.reduce_mean(0.5*tf.square(quadratic_part) + linear_part)

                # tf.summary.scalar('loss', loss)

                optimizer = self.qnet_optimizer

                grads_and_vars = optimizer.compute_gradients(loss)

                vars_with_grad = [v for g, v in grads_and_vars if g is not None]
                print(vars_with_grad)

                train_step = optimizer.apply_gradients(grads_and_vars)

            return model, loss, train_step
        else:
            return model

    def remember(self, history, state, next_state, action, reward):
        entry = tf.concat([state, next_state, tf.reshape(tf.cast(action, tf.float32), [1]), tf.reshape(reward, [1])], axis=0)
        # print(entry)
        return tf.concat([history, tf.reshape(entry, (1, -1))], axis=0)

    def act(self, state):
        cond = tf.less_equal(tf.random_uniform(shape=[1]), self.epsilon)[0]

        random_action = lambda: tf.one_hot(tf.random_uniform(shape=[1], minval=0, maxval=self.action_size, dtype=tf.int32)[0], self.action_size)

        def prediction_action():
            act_values = self.define_model(self.qnet, tf.reshape(state, (1, -1)), self.observation_size)[0]
            # act_values = tf.Print(act_values, [act_values, tf.argmax(act_values, output_type=tf.int32)], 'act_values')

            # return tf.argmax(act_values, output_type=tf.int32)  # returns action
            return act_values

        return tf.cond(cond, random_action, prediction_action)

    def predict(self, state):
        return self.define_model(self.qnet, state, self.observation_size, trainable=False)

    def target_qnet_predict(self, state):
        return self.define_model(self.target_qnet, state, self.observation_size, trainable=False)

    def replay(self, minibatch):
        #random_indexes = tf.random_shuffle(tf.range(history.shape[0]))[:self.batch_size]

        #minibatch = tf.gather(history, random_indexes)

        state = minibatch[:, 0:self.observation_size]
        next_state = minibatch[:, self.observation_size:self.observation_size * 2]
        action = tf.cast(minibatch[:, self.observation_size * 2:self.observation_size * 2 + 1], dtype=tf.int32)
        reward = tf.reshape(minibatch[:, self.observation_size * 2 + 1:self.observation_size * 2 + 2], (-1,))

        # reward = tf.Print(reward, [minibatch[0], state[0], next_state[0], action[0], reward[0]], 'action/reward: ', summarize=20)

        target_prediction = self.predict(next_state)
        target = reward + self.gamma * tf.reduce_max(target_prediction, axis=1)

        target_f = self.predict(state)

        actions = tf.concat((tf.reshape(tf.range(self.batch_size, dtype=tf.int32), (-1, 1)), action), axis=1)

        target_f += tf.scatter_nd(actions, target-tf.gather_nd(target_f, actions), target_f.shape)
        target_f = tf.stop_gradient(target_f)

        model_output, model_loss, train_step = self.define_model(self.qnet, state, self.observation_size, reference=target_f, trainable=True)

        return train_step, model_loss

    def update_epsilon(self):
        return tf.cond(tf.greater(self.epsilon, self.epsilon_min), lambda: tf.assign(self.epsilon, self.epsilon * self.epsilon_decay), lambda: self.epsilon)

    def get_embeddings(self, states):
        return self.define_model(self.qnet, states, self.observation_size, trainable=False, return_embedding=True)

    def set_test_mode(self, sess):
        self.epsilon_bak = sess.run(self.epsilon)
        sess.run([tf.assign(self.epsilon, 0.0), tf.assign(self.keep_prob, 1.0)])

    def set_train_mode(self, sess):
        sess.run([tf.assign(self.epsilon, self.epsilon_bak), tf.assign(self.keep_prob, 1 - self.dropout_rate)])

