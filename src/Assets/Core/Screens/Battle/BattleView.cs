using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Core.Battle;
using Assets.Core.Card;
using Assets.Core.Entity.Preview;
using TMPro;
using UnityEngine;

namespace Assets.Core.Screens.Battle
{
    /// <summary>
    /// Представляет интерфейс отображения пользователю
    /// </summary>
    public class BattleView : MonoBehaviour
    {
        public TMP_Text EnergyLabel;
        public string EnergyLabelFormat = "{0} / {1}";

        [Header("Cards")]
        public GameObject CardPrefab;
        public Transform CardSpawnPoint;
        public CardsLayout CardsController;
        public InvokeCardQueue CardsQueue;
        public RectTransform RevertApplyCardArea;
        public Transform CardDropPoint;
        [HideInInspector]
        public CardInstance SelectedCard;

        [Header("Entities")]
        public GameObject EntityPreviewPrefab;
        public Transform PlayerEntityPreviewParent;
        public Transform EnemyEntityPreviewParent;
        [HideInInspector]
        public EntityPreviewVisualizer SelectedEntity;
        [Range(0, 1)]
        public float MinDistanceToSelectPercentFromWidth = 0.15f;
        private float MinDistanceToSelect => Screen.width * MinDistanceToSelectPercentFromWidth;

        [Header("Links")]
        public BattleManager BattleManager;
        public CardsLayout CardsLayout;

        [Header("End battle window")]
        public GameObject EndBattleWindow;
        public GameObject EndBattleWindow_PlayerWins;
        public GameObject EndBattleWindow_PlayerLoose;

        private void Awake()
        {
            //foreach (Transform child in PlayerEntityPreviewParent)
            //    Destroy(child.gameObject);
            //foreach (Transform child in EnemyEntityPreviewParent)
            //    Destroy(child.gameObject);
            //foreach (Transform child in CardsController.CardsParent)
            //    Destroy(child.gameObject);
            EndBattleWindow.SetActive(false);
        }
        private void Update()
        {
            if (this.SelectedCard != null && this.CardsLayout?.SelectedCard != null)
            {
                IEnumerable<EntityPreviewVisualizer> Get()
                {
                    if (this.SelectedCard.data.Target.HasFlag(AcceptableTarget.Enemy))
                        foreach (Transform t in this.EnemyEntityPreviewParent)
                            if (t != null && t.TryGetComponent<EntityPreviewVisualizer>(out var visualizer))
                                yield return visualizer;

                    if (this.SelectedCard.data.Target.HasFlag(AcceptableTarget.Friend))
                        foreach (Transform t in this.PlayerEntityPreviewParent)
                            if (t != null && t.TryGetComponent<EntityPreviewVisualizer>(out var visualizer))
                                yield return visualizer;
                }


                var selectedEntity = Get()
                    .Select(x => new
                    {
                        visualizer = x,
                        distance = Vector2.Distance(x.transform.position, this.CardsLayout.SelectedCard.position)
                    })
                    .OrderBy(x => x.distance)
                    .Where(x => x.distance <= this.MinDistanceToSelect)
                    .FirstOrDefault()
                    ?.visualizer;

                this.SelectEntity(selectedEntity);
            }
        }

        #region Cards

        public CardVisualizer AddCard(CardInstance card)
        {
            var cardVisualizer = Instantiate(
                this.CardPrefab,
                this.CardSpawnPoint.position,
                this.CardSpawnPoint.rotation,
                this.CardsController.CardsParent)
                .GetComponent<CardVisualizer>();
            cardVisualizer.Show(card);
            card.visualizer = cardVisualizer;
            return cardVisualizer;
        }

        public void PushCardToQueue(CardInstance card)
        {
            if (card == this.SelectedCard)
                this.SelectCard(null);
            this.CardsQueue.Enqueue(card.visualizer.transform);
        }
        public void RemoveCard(CardInstance card)
        {
            if (card.visualizer?.transform.parent != this.CardsController.CardsParent)
                return;
            this.StartCoroutine(this.DropCard(card.visualizer));
            card.visualizer = null;
        }

        public IEnumerator DropCard(CardVisualizer visualizer)
        {
            const float dropTime = 3;
            const float minDistanceToDestroy = .1f;

            float start = Time.time;
            float end = start + dropTime;
            while (end >= Time.time && //Пока время еще не пришоло И пока мы слишком далеко от места уничтожения
                Vector2.Distance(this.CardDropPoint.position, visualizer.transform.position) > minDistanceToDestroy)
            {
                if (visualizer.transform.parent != this.CardDropPoint)
                    visualizer.transform.SetParent(this.CardDropPoint);
                visualizer.transform.position = this.CardsLayout.GetNextCardPosition(
                    visualizer.transform.position,
                    this.CardDropPoint.position);
                yield return new WaitForEndOfFrame();
            }
            Destroy(visualizer.gameObject);
        }
        public void SelectCard(CardInstance card)
        {
            if (card == null)
            {
                this.SelectedCard = null;
                this.CardsController.SelectCard(null);//Убрать карту обратно в колоду
                this.SelectEntity(null);//Сбросить выделенное существо
                return;
            }
            this.SelectedCard = card;
            this.CardsController.SelectCard(card.visualizer.transform);
        }

        public bool SelectedCardCanInvoked(out IEnumerable<Entity.Entity> targets)
        {
            targets = new List<Entity.Entity>();

            if (this.SelectedCard != null &&
                this.SelectedCard.data.Target.HasFlag(AcceptableTarget.Friend) ||
                this.SelectedCard.data.Target.HasFlag(AcceptableTarget.Enemy))
            {
                if (SelectedEntity?.entity != null)
                {
                    targets = new List<Entity.Entity>() { this.SelectedEntity?.entity };
                    return true;
                }
            }

            var localpoint = this.RevertApplyCardArea.InverseTransformPoint(this.CardsLayout.SelectedCard.position);
            bool onRevertArea = this.RevertApplyCardArea.rect.Contains(localpoint);

            if (this.SelectedCard.data.Target == AcceptableTarget.Player)
            {
                targets = new List<Entity.Entity>() { this.BattleManager.Player };
                return !onRevertArea;
            }

            if (this.SelectedCard.data.Target == AcceptableTarget.None)
            {
                return !onRevertArea;
            }

            if (this.SelectedCard.data.Target == AcceptableTarget.AllFriends)
            {
                targets = this.BattleManager.friends;
                return !onRevertArea;
            }

            if (this.SelectedCard.data.Target == AcceptableTarget.AllEnemies)
            {
                targets = this.BattleManager.enemies;
                return !onRevertArea;
            }
            return false;
        }

        public void UpdateEnergyIndicatorOfCards(IEnumerable<CardInstance> cards, int curEnergy)
        {
            foreach (var card in cards)
            {
                card.visualizer.HasEnergy = curEnergy >= card.data.BaseEnergyCost;
            }
        }

        #endregion

        #region Entities

        public EntityPreviewVisualizer AddEntity(Entity.Entity entity, bool isEnemy = false)
        {
            var entityPreviewVisualizer = Instantiate(
                this.EntityPreviewPrefab,
                isEnemy ? this.EnemyEntityPreviewParent : this.PlayerEntityPreviewParent)
                .GetComponent<EntityPreviewVisualizer>();
            entityPreviewVisualizer.Show(entity);
            return entityPreviewVisualizer;
        }

        public void RemoveEntity(Entity.Entity entity)
        {
            if (this.FindEntity(entity, out var visualizer))
            {
                Destroy(visualizer.gameObject);
            }
        }

        public bool FindEntity(Entity.Entity entity, out EntityPreviewVisualizer visualizer)
        {
            bool FindEntity(Transform parent, Entity.Entity entity, out EntityPreviewVisualizer visualizer)
            {
                foreach (Transform child in parent)
                {
                    visualizer = child.GetComponentInChildren<EntityPreviewVisualizer>();
                    if (visualizer != null && entity.Equals(visualizer.entity))
                    {
                        return true;
                    }
                }
                visualizer = null;
                return false;
            }
            return
                FindEntity(this.PlayerEntityPreviewParent, entity, out visualizer) ||
                FindEntity(this.EnemyEntityPreviewParent, entity, out visualizer);
        }

        void SelectEntity(EntityPreviewVisualizer visualizer)
        {
            if (this.SelectedEntity != null)
                this.SelectedEntity.IsSelected = false;
            this.SelectedEntity = visualizer;
            if (this.SelectedEntity != null)
            {
                this.SelectedEntity.IsSelectFriend = this.PlayerEntityPreviewParent
                    .GetEnumerator()
                    .ToEnumerable()
                    .Select(x => (Transform)x)
                    .Any(x => Equals(x, visualizer.transform));
                this.SelectedEntity.IsSelected = true;
            }
        }

        public void SetNowIsStepping(Entity.Entity entity, bool nowIsStepping)
        {
            if (this.FindEntity(entity, out var visualizer))
                visualizer.NowIsStepping = nowIsStepping;
        }

        #endregion

        public void UpdateEnergyLabel(int CurEnergy, int MaxEnergy)
        {
            this.EnergyLabel.text = string.Format(this.EnergyLabelFormat, CurEnergy, MaxEnergy);
        }
        public void ShowEndBattleWindow(bool isPlayerWins)
        {
            this.EndBattleWindow.SetActive(true);
            this.EndBattleWindow_PlayerWins.SetActive(isPlayerWins);
            this.EndBattleWindow_PlayerLoose.SetActive(!isPlayerWins);
        }
    }
}
