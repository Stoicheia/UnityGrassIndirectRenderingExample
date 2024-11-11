using System.Collections.Generic;
using System.Linq;
using AudioEngine.Music;
using MagicGrass.SoundPads;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace MagicGrass.ProcGen.PadPlacer
{
    public class PadPlacer : SerializedMonoBehaviour
    {
        [SerializeField] private SoundPad _padPrefab;
        [SerializeField] private SoundPadManager _padManager;
        [SerializeField] private Transform _origin;
        [SerializeField] private float _padScale;
        [SerializeField] private float _padSlotSize;
        [SerializeField] private TextAsset _chordMap;
        [SerializeField] private Vector2Int _gridDimensions;
        private FunctionalChord[,] _chordGrid;

        [Button]
        public void Generate()
        {
            ReadChordMap(_chordMap);
            GeneratePadGrid();
        }

        [Button]
        public void ReadChordMap(TextAsset map)
        {
            string text = map.text;
            string[] lines = text.Split('\n');
            FunctionalChord[,] chordGrid = new FunctionalChord[_gridDimensions.x, _gridDimensions.y];
            List<List<FunctionalChord>> chordTableau = new List<List<FunctionalChord>>();
            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                List<FunctionalChord> newRow = new List<FunctionalChord>();
                foreach (string chordName in parts)
                {
                    string c = chordName.Trim();
                    FunctionalChord chord = ChordUtility.ChordNameToChord(c);
                    newRow.Add(chord);
                }
                chordTableau.Add(newRow);
            }
            
            
            for (int y = 0; y < _gridDimensions.y; y++)
            {
                for (int x = 0; x < _gridDimensions.x; x++)
                {
                    chordGrid[x, y] = chordTableau[x][y];
                }
            }

            _chordGrid = chordGrid;
        }

        [Button]
        public void GeneratePadGrid()
        {
            var tempList = _origin.Cast<Transform>().ToList();
            foreach(var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }

            float scaledSlotSize = _padSlotSize * _padScale;
            for (int y = 0; y < _gridDimensions.y; y++)
            {
                for (int x = 0; x < _gridDimensions.x; x++)
                {
                    FunctionalChord myChord = _chordGrid[x, y];
                    Vector2 offset = scaledSlotSize*new Vector2(x, y) + scaledSlotSize*Vector2.one/2;
                    Vector3 position = new Vector3(offset.x, transform.position.y, offset.y) + _origin.position;
                    SoundPad padInstance = Instantiate(_padPrefab, position, transform.rotation, _origin);
                    padInstance.transform.localScale = Vector3.one * _padScale;
                    padInstance.Chord = myChord;
                    padInstance.name = myChord.ToString();
                }
            }
            
            _padManager.UpdatePads();
        }
    }
}