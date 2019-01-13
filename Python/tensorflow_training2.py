import math
import os
import time

import tensorflow as tf
from gym_unity.envs import UnityEnv
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from tensorflow.python.tools import freeze_graph
import pickle

from tensorflow_training import plot_observations

result_dir = 'results2'

def take_action(observation, observation_header_size, step):
    w = int(observation[1])
    h = int(observation[2])
    grid = observation[observation_header_size:].reshape(w, h)

    enemies = []

    for y in range(h):
        for x in range(w):
            if grid[x, y] > 0:
                enemies.append([x-w/2, y-h/2])

    if len(enemies) < 1:
        return 3

    enemy = enemies[0]
    angle = math.degrees(math.atan2(enemy[0], enemy[1]))

    print(angle)

    if step % 3 == 0:
        return 0

    plot_observations([observation], observation_header_size)

    if angle < -10:
        return 4
    elif angle > 10:
        return 3
    else:
        return 0

def main():
    env = UnityEnv("../Build/BendersTemp.exe", 0, use_visual=False, multiagent=True, no_graphics=False)

    observation_header_size = 4
    observation_size = len(env.observation_space.low)
    action_size = len(env.action_space.low)

    observations = env.reset()

    num_agents = len(observations)

    for step in range(1000):
        action_vecs = np.zeros((num_agents, action_size))

        for agent_idx in range(1):
            action = take_action(observations[agent_idx], observation_header_size, step)
            action_vecs[agent_idx, action] = 1

        observations, rewards, done, info = env.step(list(action_vecs))

        time.sleep(0.01)


if __name__ == '__main__':
    if not os.path.exists(result_dir):
        os.mkdir(result_dir)

    main()

