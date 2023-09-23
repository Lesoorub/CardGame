using System.Collections.Generic;
using System.Linq;
using Assets.Core.Battle;
using Assets.Core.Card;
using Assets.Core.Entity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Core.Screens.Battle
{
    /// <summary>
    /// Обрабатывает взаимодействия пользователя с моделью
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        public BattleModel model;
        public BattleView view;
        public string MapSceneName;

        void OnEnable()
        {
            this.model.Cards.Added += this.Cards_OnAdded;
            this.model.Cards.Removed += this.Cards_OnRemoved;

            this.model.EnemyEntities.Added += this.EnemyEntities_OnAdded;
            this.model.EnemyEntities.Removed += this.EnemyEntities_OnRemoved;

            this.model.PlayerEntities.Added += this.PlayerEntities_OnAdded;
            this.model.PlayerEntities.Removed += this.PlayerEntities_OnRemoved;

            this.view?.BattleManager?.OnEntityAdded.AddListener(this.BattleManager_OnEntityAdded);
            this.view?.BattleManager?.OnEntityRemoved.AddListener(this.BattleManager_OnEntityRemoved);
            this.view?.BattleManager?.OnStepBegins.AddListener(this.BattleManager_OnStepBegins);
            this.view?.BattleManager?.OnStepEnded.AddListener(this.BattleManager_OnStepEnded);
            this.view?.BattleManager?.OnBattleBegins.AddListener(this.BattleManager_OnBattleBegins);
            this.view?.BattleManager?.OnBattleEnded.AddListener(this.BattleManager_OnBattleEnded);

            this.model.OnEnergyChanged += this.Model_OnEnergyChanged;
        }

        private void OnDisable()
        {
            this.model.Cards.Added -= this.Cards_OnAdded;
            this.model.Cards.Removed -= this.Cards_OnRemoved;

            this.model.EnemyEntities.Added -= this.EnemyEntities_OnAdded;
            this.model.EnemyEntities.Removed -= this.EnemyEntities_OnRemoved;

            this.model.PlayerEntities.Added -= this.PlayerEntities_OnAdded;
            this.model.PlayerEntities.Removed -= this.PlayerEntities_OnRemoved;

            this.view?.BattleManager?.OnEntityAdded.RemoveListener(this.BattleManager_OnEntityAdded);
            this.view?.BattleManager?.OnEntityRemoved.RemoveListener(this.BattleManager_OnEntityRemoved);
            this.view?.BattleManager?.OnStepBegins.RemoveListener(this.BattleManager_OnStepBegins);
            this.view?.BattleManager?.OnStepEnded.RemoveListener(this.BattleManager_OnStepEnded);
            this.view?.BattleManager?.OnBattleBegins.RemoveListener(this.BattleManager_OnBattleBegins);
            this.view?.BattleManager?.OnBattleEnded.RemoveListener(this.BattleManager_OnBattleEnded);

            this.model.OnEnergyChanged -= this.Model_OnEnergyChanged;
        }

        #region handles

        #region Entity handles
        private void PlayerEntities_OnAdded(IEnumerable<Entity.Entity> changes)
        {
            foreach (var c in changes)
                this.view.AddEntity(c);
        }
        private void PlayerEntities_OnRemoved(IEnumerable<Entity.Entity> changes)
        {
            foreach (var c in changes)
                this.view.RemoveEntity(c);
        }
        private void EnemyEntities_OnAdded(IEnumerable<Entity.Entity> changes)
        {
            foreach (var c in changes)
                this.view.AddEntity(c, true);
        }
        private void EnemyEntities_OnRemoved(IEnumerable<Entity.Entity> changes)
        {
            foreach (var c in changes)
                this.view.RemoveEntity(c);
        }
        private void BattleManager_OnEntityAdded(BattleManager battleManager, EntityArgs args)
        {
            var entity = args.entity;
            if (args.isFriend)
            {
                this.model.PlayerEntities.Add(entity);
                entity.Cards.OnChanged += this.Cards_OnChanged;
                entity.Cards.Hand.Added += this.Hand_OnAdded;
                entity.Cards.Hand.Removed += this.Hand_OnRemoved;
                this.model.Cards.AddRange(entity.Cards.Hand);
                this.Cards_OnChanged(entity.Cards);
            }
            if (!args.isFriend)
            {
                this.model.EnemyEntities.Add(entity);
            }
        }

        private void Hand_OnAdded(IEnumerable<CardInstance> changes)
        {
            this.model.Cards.AddRange(changes);
        }

        private void Hand_OnRemoved(IEnumerable<CardInstance> changes)
        {
            foreach (var c in changes)
                this.model.Cards.Remove(c);
        }

        private void Cards_OnChanged(CardContainer cardManager)
        {
            this.model.CurEnergy = cardManager.CurEnergy;
            this.model.MaxEnergy = cardManager.MaxEnergy;
        }

        private void BattleManager_OnEntityRemoved(BattleManager battleManager, EntityArgs args)
        {
            var entity = args.entity;
            if (args.isFriend)
            {
                this.model.PlayerEntities.Remove(entity);
                entity.Cards.OnChanged -= this.Cards_OnChanged;
                entity.Cards.Hand.Added -= this.Hand_OnAdded;
                entity.Cards.Hand.Removed -= this.Hand_OnRemoved;
            }
            if (!args.isFriend)
            {
                this.model.EnemyEntities.Remove(entity);
            }
        }
        #endregion

        #region Card handles
        private void Cards_OnAdded(IEnumerable<CardInstance> changes)
        {
            foreach (var c in changes)
            {
                var visualizer = this.view.AddCard(c);
                visualizer.OnClickDown.AddListener(this.Card_OnClickDown);
                visualizer.OnClickUp.AddListener(this.Card_OnClickUp);
                var player = this.view.BattleManager.Player;
                if (player != null)
                {
                    visualizer.HasEnergy = player.Cards.CurEnergy >= c.data.BaseEnergyCost;
                }
            }
        }
        private void Cards_OnRemoved(IEnumerable<CardInstance> changes)
        {
            foreach (var c in changes)
            {
                this.view.RemoveCard(c);
                c.visualizer?.OnClickDown.RemoveListener(this.Card_OnClickDown);
                c.visualizer?.OnClickUp.RemoveListener(this.Card_OnClickUp);
            }
        }
        private void Card_OnClickDown(CardVisualizer visualizer)
        {
            this.view.SelectCard(visualizer.card);
        }
        private void Card_OnClickUp(CardVisualizer visualizer)
        {
            if (this.view.SelectedCardCanInvoked(out var targets))
            {
                var player = this.view.BattleManager.Player;
                var card = this.view.SelectedCard;
                if (player != null &&
                    this.view.BattleManager.CanInvokeCard(card, player, targets))
                {
                    this.view.PushCardToQueue(card);
                    this.view.BattleManager.InvokeCard(card, player, targets);
                    this.view.UpdateEnergyIndicatorOfCards(this.model.Cards, player.Cards.CurEnergy);
                }
            }
            this.view.SelectCard(null);
        }
        #endregion

        #region Energy

        private void Model_OnEnergyChanged(int MaxEnergy, int CurEnergy)
        {
            this.view.UpdateEnergyLabel(CurEnergy, MaxEnergy);
        }

        #endregion

        /// <summary>
        /// Вызывается во время конца хода
        /// </summary>
        /// <param name="battleManager">Менеджер битвы</param>
        private void BattleManager_OnStepEnded(BattleManager battleManager)
        {
            this.model.Cards.Clear();
        }
        /// <summary>
        /// Вызывается в начале хода
        /// </summary>
        /// <param name="battleManager">Менеджер битвы</param>
        private void BattleManager_OnStepBegins(BattleManager battleManager)
        {
            this.model.CurEnergy = battleManager.Player.Cards.CurEnergy;
            this.model.MaxEnergy = battleManager.Player.Cards.MaxEnergy;
            this.UpdateIsNowSteppingForAll();
        }


        /// <summary>
        /// Вызывается перед первым ходом в битвы
        /// </summary>
        /// <param name="battleManager">Менеджер битвы</param>
        private void BattleManager_OnBattleBegins(BattleManager battleManager)
        {
            this.UpdateIsNowSteppingForAll();
        }
        /// <summary>
        /// Вызывается когда битва была окончена
        /// </summary>
        /// <param name="battleManager">Менеджер битвы</param>
        private void BattleManager_OnBattleEnded(BattleManager battleManager, bool isFirstSideWins)
        {
            this.view.ShowEndBattleWindow(isFirstSideWins);
            foreach (var entity in this.model.PlayerEntities.Concat(this.model.EnemyEntities))
                this.view.SetNowIsStepping(entity, false);
        }

        #endregion

        public void _EndTurn()
        {
            if (this.view.BattleManager.WhoNowSteping == this.view.BattleManager.Player)
                this.view.BattleManager._EndOfStep();
        }
        public void _BackToMap()
        {
            SceneManager.LoadScene(this.MapSceneName);
        }

        void UpdateIsNowSteppingForAll()
        {
            var who = this.view.BattleManager.WhoNowSteping;
            foreach (var entity in this.model.PlayerEntities.Concat(this.model.EnemyEntities))
            {
                this.view.SetNowIsStepping(entity, Equals(entity, who));
            }
        }
    }
}