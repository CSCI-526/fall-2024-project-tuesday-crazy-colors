
# Crazy Colors

Logline: A 2D platformer game where the player jumps on different color platforms where the player must changes colors in mid air to match the next platform’s color.

## Setup

1. Create an empty folder called 'Crazy Colors'
2. Open it in cmd line
3. git init
4. git branch -m master main
5. git remote add origin https://github.com/CSCI-526/fall-2024-project-tuesday-crazy-colors.git
6. git pull origin main
7. git branch --set-upstream-to origin/main


## Alpha Build:
https://devkansara.github.io/Crazy-Colors-Alpha-Progress/

## GDD: 
https://docs.google.com/document/d/1q6PHDWefOv1Gqsp_OpkrI-I6fdMRVhv2ZSCtKIFEobw/edit?usp=sharing

## Genre: 
2D-Platformer, Endless game

## Twist:  
Single Player Color Switch

## Description:
The game involves a player that can endlessly move ahead/behind over vertically oscillating platforms and switch the color of the player to match the succeeding platform’s color. If the player lands on a platform that does not match its own color, the player dies. It has a shadow that keeps following the player in order to kill it. The shadow mimics all the movements of the player. If the shadow manages to catch the player, the player dies. The player scores one point after crossing one platform. The player gets a white power-up giving it the opportunity to be immune to different color platforms for 10 seconds. At the end of the power-up time, the player reverts back to its original color that might not match the current platform’s color. The player can also change color during the power up time period to get ready for the next platform to jump on when the timer turns zero. The objective of the game is to collect as many points by switching the player’s color and landing on the equivalent platforms while being ahead of the shadow.

## Controls:
- Movement: A/left arrow key for left movement, D/right arrow key for right movement
- Jump: W/top arrow key
- Player Color Switch: Space
- Color Sequence: Red -> Green -> Yellow
