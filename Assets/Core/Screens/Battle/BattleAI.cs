using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Core.Battle;
using UnityEngine;

namespace Assets.Core.Screens.Battle
{

    public class BattleAI : MonoBehaviour
    {
        public BattleManager battleManager;
        public BattleView view;
        public Transform CardsSpawnPoint;

        private void OnEnable()
        {
            this.battleManager?.OnStepBegins?.AddListener(this.battleManager_OnStepBegins);
            this.battleManager?.OnBattleBegins?.AddListener(this.battleManager_OnBattleBegins);
        }
        private void OnDisable()
        {
            this.battleManager?.OnStepBegins?.RemoveListener(this.battleManager_OnStepBegins);
            this.battleManager?.OnBattleBegins?.RemoveListener(this.battleManager_OnBattleBegins);
        }

        private void battleManager_OnBattleBegins(BattleManager battleManager)
        {

        }
        private void battleManager_OnStepBegins(BattleManager battleManager)
        {
            if (battleManager.WhoNowSteping == battleManager.Player)
            {
                return;
            }
            var who = battleManager.WhoNowSteping;

            var friends = battleManager.friends.Contains(who) ? battleManager.friends : battleManager.enemies;
            var enemies = battleManager.friends.Contains(who) ? battleManager.enemies : battleManager.friends;

            var steps =
                who.ai.CalculateStep(who, battleManager, friends, enemies);

            this.StartCoroutine(this.ProcessAIStep(who, steps));
        }

        IEnumerator ProcessAIStep(
            Entity.Entity who,
            IEnumerable<(Card.CardInstance card, IEnumerable<Entity.Entity> targets)> steps)
        {
            const float WaitBeforeAttack = 0.5f;
            const float WaitBetweenAttacks = 1f;
            const float WaitAfterAttack = 0.5f;

            yield return new WaitForSeconds(WaitBeforeAttack);
            foreach (var step in steps)
            {
                Debug.Log($"card={step.card.data.name} targets=[{string.Join(", ", step.targets.Select(x => $"{{name={x.Data.name}}}"))}]");
                this.battleManager.InvokeCard(step.card, who, step.targets);
                var visualizer = this.view.AddCard(step.card);
                step.card.visualizer = visualizer;
                visualizer.transform.SetParent(this.transform);
                if (this.view.FindEntity(who, out var ent_visualizer))
                    visualizer.transform.position = ent_visualizer.transform.position;
                else
                    visualizer.transform.position = this.CardsSpawnPoint.position;
                this.view.PushCardToQueue(step.card);
                yield return new WaitForSeconds(WaitBetweenAttacks);
            }
            yield return new WaitForSeconds(WaitAfterAttack);
            this.battleManager._EndOfStep();
        }
    }
}