using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Application.GUI.WindowsSystem;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Project.Gameplay.TruePuzzle.TubeGame
{
    public class TubeGameWindow : WindowBase
    {
        public Subject<Unit> OnComplete { get; } = new();
        [SerializeField] private GridLayoutGroup _grid;

        private TubeCellView[] _cells;

        private void Awake()
        {
            _cells = _grid.GetComponentsInChildren<TubeCellView>();

            foreach (var cell in _cells)
            {
                cell.Init(this);
            }

            RandomizePipes();
        }

        public void RandomizePipes()
        {
            foreach (var cell in _cells)
            {
                for (var i = 0; i < Random.Range(0, 5); i++)
                {
                    cell.UpdateVisual();
                }
            }

            if (IsVictory())
            {
                RandomizePipes();
            }
        }

        public void OnCellClicked(TubeCellView cell)
        {
            cell.UpdateVisual();
            if (IsVictory())
            {
                OnComplete?.OnNext(Unit.Default);
            }
        }

        public bool IsVictory()
        {
            return _cells.All(cell => cell.IsCorrect());
        }
    }
}