import pandas as pd
import matplotlib.pyplot as plt

file_path = 'Game_analytics.csv'  
df = pd.read_csv(file_path)

df['Timestamp'] = pd.to_datetime(df['Timestamp'])

print(df.head())

# Plot Scatter Plot: Score vs Gameplay Time
plt.figure(figsize=(10, 6))
plt.scatter(df['gamePlayTime'], df['score'], color='blue')
plt.title('Score vs Gameplay Time')
plt.xlabel('Gameplay Time (seconds)')
plt.ylabel('Score')
plt.grid(True)
plt.tight_layout()
plt.show()

# Calculate the correlation coefficient
correlation = df['gamePlayTime'].corr(df['score'])
print(f"Correlation coefficient: {correlation:.2f}")
