using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
	//player and enemy's game objects
	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	GameObject playerGO;

	//buttons
	public GameObject specialButton;
	public GameObject attackButton;
	public GameObject healButton;

	//places where the characters are anchored to
	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	//animators
	animControlBridge anim;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;
	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

    // Start is called before the first frame update
    void Start()
    {
		Disable();
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();
		//anim = playerGO.GetComponent<animControlBridge>();    //gets the animator from the player prefab

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		Enable();
		PlayerTurn();
	}

	IEnumerator PlayerAttack()
	{
		//anim.setBoolean("isAttacking", true);

		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "big oof";

		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
			SetIsAttacking(false);
			EndBattle();
		} else
		{
			state = BattleState.ENEMYTURN;
			Disable();
			SetIsAttacking(false);
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator PlayerUltraKill()
	{
		bool isDead = enemyUnit.TakeDamage(playerUnit.damage * playerUnit.multiplier);

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "BAH BAAAMN";

		yield return new WaitForSeconds(2f);

		if (isDead)
		{
			SetIsAttacking(false);
			state = BattleState.WON;
			EndBattle();
		}
		else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn()
	{
		dialogueText.text = enemyUnit.unitName + " attacks!";

		yield return new WaitForSeconds(1f);

		bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(1f);

		if(isDead)
		{
			SetIsAttacking(false);
			state = BattleState.LOST;
			EndBattle();
		} else
		{
			state = BattleState.PLAYERTURN;
			Enable();
			PlayerTurn();
		}

	}

	void EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
		}
	}

	void PlayerTurn()
	{
		dialogueText.text = "Choose an action:";
	}

	IEnumerator PlayerHeal()
	{
		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "You feel renewed strength!";

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		SetIsAttacking(true);
		StartCoroutine(PlayerAttack());
	}

	public void OnUltralKillButton()
    {
		if (state != BattleState.PLAYERTURN)
			return;

		SetIsAttacking(true);
		StartCoroutine(PlayerUltraKill());
    }

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerHeal());
	}

	
	public void Disable()
    {
		specialButton.SetActive(false);
		attackButton.SetActive(false);
		healButton.SetActive(false);
    }

	public void Enable()
    {
		specialButton.SetActive(true);
		attackButton.SetActive(true);
		healButton.SetActive(true);
	}
	
	//sets the value of isAttacking
	public void SetIsAttacking(bool value)
    {
		//sets the animator to the prfabs animator
		anim = playerGO.GetComponent<animControlBridge>();    //gets the animator from the player prefab

		//method in the animControl script
		anim.SetIsAttacking(value);
    }

}
