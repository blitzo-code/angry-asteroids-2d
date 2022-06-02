using System;
using System.Collections.Generic;
using AngryAsteroids2D.Source.Data.Level;
using UnityEditor;
using UnityEngine;

namespace AngryAsteroids2D.Source.Data.Editor
{
    [CustomEditor(typeof(LevelConfigAsset))]
    public class LevelConfigAssetEditor : UnityEditor.Editor
    {
        SerializedObject _so;
        SerializedProperty _levelWavesProperty;

        WaveData _waveData;
        WaveType _waveType;
        AsteroidType _asteroidType;
        Vector3 _spawnPosition;
        Vector3 _angleVector;
        float _spawnDirection;
        

        void OnEnable()
        {
            _so = serializedObject;
            _levelWavesProperty = _so.FindProperty("LevelWaves");
            SceneView.duringSceneGui += DuringSceneGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            _so.Update();
            
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.Separator();
                
              
                _waveType = (WaveType) EditorGUILayout.EnumPopup("Wave Type", _waveType);
                _asteroidType = (AsteroidType) EditorGUILayout.EnumPopup("Asteroid Type", _asteroidType);
                _spawnDirection = EditorGUILayout.FloatField("Movement Direction Angle", _spawnDirection);
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Actions");
                
                if (GUILayout.Button("Create new wave"))
                {
                    _waveData = new WaveData();
                    _waveData.Asteroids = new List<AsteroidSpawnData>();
                }

                if (GUILayout.Button("Add asteroid to current wave"))
                {
                    if (_waveData.Asteroids == null)
                    {
                        _waveData.Asteroids = new List<AsteroidSpawnData>();
                    }
                    
                    _waveData.Asteroids.Add(new AsteroidSpawnData
                    {
                        SpawnPoint = _spawnPosition,
                        MoveDirection = (_angleVector - _spawnPosition).normalized,
                        AsteroidType = _asteroidType
                    });
                }

                if (GUILayout.Button("Commit wave"))
                {
                    _levelWavesProperty.InsertArrayElementAtIndex(_levelWavesProperty.arraySize);
                    
                    var insertedWave = _levelWavesProperty.GetArrayElementAtIndex(_levelWavesProperty.arraySize - 1);
                    insertedWave.FindPropertyRelative("WaveType").enumValueIndex = (int)_waveType;
                    
                    var spawnDataCollection = insertedWave.FindPropertyRelative("Asteroids");
                    spawnDataCollection.ClearArray();
                    
                    for (var i = 0; i < _waveData.Asteroids.Count; i++)
                    {
                        spawnDataCollection.InsertArrayElementAtIndex(i);
                        
                        var spawnData = spawnDataCollection.GetArrayElementAtIndex(i);
                        spawnData.FindPropertyRelative("SpawnPoint").vector3Value = _waveData.Asteroids[i].SpawnPoint;
                        spawnData.FindPropertyRelative("MoveDirection").vector3Value = _waveData.Asteroids[i].MoveDirection;
                        spawnData.FindPropertyRelative("AsteroidType").enumValueIndex = (int) _waveData.Asteroids[i].AsteroidType;
                    }
                }
            }
            
            _so.ApplyModifiedProperties();
        }

        void DuringSceneGUI(SceneView sceneView)
        {
            _so.Update();

            Handles.Label(_spawnPosition, "Spawn position");
            _spawnPosition = Handles.PositionHandle(_spawnPosition, Quaternion.identity);

            _angleVector.x = _spawnPosition.x + 5 * Mathf.Sin(_spawnDirection * Mathf.Deg2Rad);
            _angleVector.y = _spawnPosition.y + 5 * Mathf.Cos(_spawnDirection * Mathf.Deg2Rad); 
            
            Handles.color = Color.red;
            
            Handles.DrawLine(_spawnPosition, _angleVector, 2);
            Handles.DrawWireDisc(_spawnPosition, Vector3.forward, 5);
            
            var createdAsteroids = _waveData.Asteroids;
            if (createdAsteroids != null)
            {
                for (var i = 0; i < _waveData.Asteroids.Count; i++)
                {
                    var asteroid = createdAsteroids[i];
                    Handles.DrawLine(asteroid.SpawnPoint, asteroid.SpawnPoint + asteroid.MoveDirection * 5, 2);
                    Handles.DrawWireDisc(asteroid.SpawnPoint, Vector3.forward, 5);
                }
            }
            
            sceneView.Repaint();
            
            _so.ApplyModifiedProperties();
        }
    }
}
