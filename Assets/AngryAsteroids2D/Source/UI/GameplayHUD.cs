using System;
using AngryAsteroids2D.Source.Gameplay.Level;
using AngryAsteroids2D.Source.Gameplay.Player;
using UnityEngine;
using UnityEngine.UI;

namespace AngryAsteroids2D.Source.UI
{
    /*
     * As we just have simple HUD elements
     * I've chosen to not code a UIFramework, let's just use a MonoBehaviour instead
     */
    
    public class GameplayHUD : MonoBehaviour
    {
        [SerializeField] Text scoreLabel;
        [SerializeField] Text livesLabel;
        [SerializeField] Text wavesLabel;

        EnemyWaveManager _enemyWaveManager;
        PlayerDataManager _playerDataManager;
        
        public void LinkEvents(EnemyWaveManager enemyWaveManager, PlayerDataManager playerData)
        {
            _enemyWaveManager = enemyWaveManager;
            _playerDataManager = playerData;
            
                
            _playerDataManager.ScoreChanged += ChangeScoreValue;
            _playerDataManager.LivesChanged += ChangeLivesValue;
            _enemyWaveManager.WaveChanged += ChangeWaveLabel;
        }

        void OnDestroy()
        {
            if (_playerDataManager != null)
            {
                _playerDataManager.ScoreChanged -= ChangeScoreValue;
                _playerDataManager.LivesChanged -= ChangeLivesValue;
            }

            if (_enemyWaveManager != null)
            {
                _enemyWaveManager.WaveChanged -= ChangeWaveLabel;
            }
        }

        void ChangeWaveLabel(int currentWave)
        {
            wavesLabel.text = $"Wave {currentWave + 1}";
        }

        void ChangeLivesValue(int lives)
        {
            livesLabel.text = $"x{lives.ToString()}" ;
        }

        void ChangeScoreValue(int value)
        {
            scoreLabel.text = value.ToString();
        }
    }
}
