using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//version of Battle System that uses an "Ok" button instead of timers

public enum BattleStateOK { START, PLAYER_TURN, ENEMY_TURN, WON, LOST, JOHN_IS_JOINING }

public class BattleSystemOKver : MonoBehaviour
{
	//player and enemy's game objects
	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	//John's and Vincent's prefabs and animators
	public GameObject johnPrefab;

	//character's game objects
	GameObject playerGO;
	GameObject johnGO;

	//buttons
	public GameObject attackButton;
	public GameObject healButton;
	public GameObject okButton;

	//places where the characters are anchored to
	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	//animators
	//animControlBridge anim;
	animControl anim;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;
	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleStateOK state;

	public int turnNum;

	bool johnIn;

	bool enemyIsDead;
	bool playerIsDead;

    // Start is called before the first frame update
    void Start()
    {
		state = BattleStateOK.START;
		SetupBattle();
    }

	void SetupBattle()
	{
		//sets up John
		disableJohn();
		johnIn = false;

		turnNum = 1;

		//isDead = false;

		//sets up the player
		playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();

		//sets up the enemy
		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		DisableButtons();

		dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		//wait for Ok button
	}

	void PlayerAttack()
	{
		//Debug.Log(turnNum);

		DisableButtons();

		enemyIsDead = enemyUnit.TakeDamage(playerUnit.damage);

		enemyHUD.SetHP(enemyUnit.currentHP);

		dialogueText.text = "Big Oof";

		EnableOkButton();

		//wait for Ok button
	}

	void EnemyTurn()
	{
		Debug.Log("Enemy attacked");

		turnNum++;

		dialogueText.text = enemyUnit.unitName + " attacks!";

		playerIsDead = playerUnit.TakeDamage(enemyUnit.damage);

		playerHUD.SetHP(playerUnit.currentHP);

		//wait for Ok button
	}

	void EndBattle()
	{
		DisableButtons();
		DisableOkButton();

		if(state == BattleStateOK.WON)
		{
			dialogueText.text = "You won the battle!";
		} else if (state == BattleStateOK.LOST)
		{
			dialogueText.text = "You were defeated.";
		}
	}

	void PlayerTurn()
	{
		Debug.Log("player turn");

		DisableOkButton();

		state = BattleStateOK.PLAYER_TURN;

		EnableButtons();

		dialogueText.text = "Choose an action:";

		//waits for an action
	}

	void PlayerHeal()
	{
		DisableButtons();

		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "You feel renewed strength!";

		EnableOkButton();

		//wait for Ok button
	}

	public void OnAttackButton()
	{
		Debug.Log("Casey attacked");

		SetIsAttacking(true);
		PlayerAttack();
	}

	public void OnHealButton()
	{
		SetIsAttacking(true);
		PlayerHeal();
	}

	
	public void DisableButtons()
    {
		attackButton.SetActive(false);
		healButton.SetActive(false);
    }

	public void EnableButtons()
    {
		attackButton.SetActive(true);

        if (johnIn)
        {
			healButton.SetActive(true);
		}

	}
	
	//sets the value of isAttacking
	public void SetIsAttacking(bool value)
    {
		//sets the animator to the prfabs animator
		anim = playerGO.GetComponent<animControl>();    //gets the animator from the player prefab

		//method in the animControl script
		anim.SetIsAttacking(value);
    }

	public void disableJohn()
    {
		Debug.Log("JA is disabled");

		johnPrefab.SetActive(false);
	}

    void EnableJohn()
    {
		johnPrefab.SetActive(true);

		johnIn = true;

		dialogueText.text = "John Alexander Warnock, \"aka My Human\", joins the battle! ";

		state = BattleStateOK.JOHN_IS_JOINING;

		//wait for Ok button
	}

	public void OnOkButton()
    {
		Debug.Log("Ok");

		//for the start
		if(state == BattleStateOK.START)
        {
			PlayerTurn();

			EnableButtons();

			return;
		}

		//for the player attack
		if(state == BattleStateOK.PLAYER_TURN)
        {

			Debug.Log("Ok (player turn)");

			if (enemyIsDead)
			{
				state = BattleStateOK.WON;
				SetIsAttacking(false);
				EndBattle();
			}
			else
			{
				state = BattleStateOK.ENEMY_TURN;
				DisableButtons();
				SetIsAttacking(false);
				EnemyTurn();
			}

			return;
		}

		//for the enemy turn
		if(state == BattleStateOK.ENEMY_TURN)
        {

			if (playerIsDead)
			{
				SetIsAttacking(false);
				state = BattleStateOK.LOST;
				EndBattle();
			}
			else
			{

				if (turnNum == 3)
				{
					EnableJohn();

					return;
				}
				else
				{
					state = BattleStateOK.PLAYER_TURN;
					//Enable();
					PlayerTurn();
				}
			}

			return;
		}

		//when John joins the battle
		if( state == BattleStateOK.JOHN_IS_JOINING)
        {
			state = BattleStateOK.PLAYER_TURN;
			PlayerTurn();
		}

	}

	public void EnableOkButton()
    {
		okButton.SetActive(true);
    }

	public void DisableOkButton()
    {
		okButton.SetActive(false);
    }

}
