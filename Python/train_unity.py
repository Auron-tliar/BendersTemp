import gym

from baselines import deepq
from gym_unity.envs import UnityEnv
from random import randint
import time
import numpy as np


def main():
    env = UnityEnv("../Build/BendersTemp.exe", 0, use_visual=False)
    # model = deepq.models.cnn_to_mlp(
    #     convs=[(32, 8, 4), (64, 4, 2), (64, 3, 1)],
    #     hiddens=[256],
    #     dueling=True,
    # )
    # act = deepq.learn(
    #     env,
    #     q_func=model,
    #     lr=1e-3,
    #     max_timesteps=100000,
    #     buffer_size=50000,
    #     exploration_fraction=0.1,
    #     exploration_final_eps=0.02,
    #     print_freq=10,
    # )

    env.action_space.n = len(env.action_space.low)
    print(env.action_space.n)


    act = deepq.learn(
        env,
        network='mlp',
        lr=1e-3,
        total_timesteps=100000,
        buffer_size=50000,
        exploration_fraction=0.1,
        exploration_final_eps=0.02,
        print_freq=10
    )

    print("Saving model to unity_model.pkl")
    act.save("unity_model.pkl")


def perform_random_actions():
    env = UnityEnv("../Build/BendersTemp.exe", 0, use_visual=False)

    env.reset()

    action_size = 8

    for i in range(0, 1000):
        action = randint(0, action_size-1)
        print(action)
        action_vec = np.zeros(action_size)
        action_vec[action] = 1
        reward = env.step(action_vec)
        print('reward: '+str(reward))

    env.reset()


if __name__ == '__main__':
    #main()
    perform_random_actions()

