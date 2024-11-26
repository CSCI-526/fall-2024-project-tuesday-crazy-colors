import pandas as pd
import matplotlib.pyplot as plt

file_path = 'Game_analytics.csv' 
df = pd.read_csv(file_path)

df['Timestamp'] = pd.to_datetime(df['Timestamp'])

plt.figure(figsize=(10, 6))
plt.bar(df['gamePlayTime'], df['score'], color='blue', width=1.0)

# Customizing the plot
plt.title('Score vs Gameplay Time')
plt.xlabel('Gameplay Time (seconds)')
plt.ylabel('Score')
plt.tight_layout()

# Display the plot
plt.show()
