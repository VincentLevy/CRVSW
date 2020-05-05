using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//version of Battle System that uses an "Ok" button instead of timers

public enum BattleStateOK { START, PLAYER_TURN, ENEMY_TURN, WON, LOST, JOHN_IS_JOINING, MESSAGE }

public class BattleSystemOKver : MonoBehaviour
{
	//player and enemy's game objects
	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	//John's and Vincent's prefabs and animators
	public GameObject johnPrefab;
	public GameObject vincentPrefab;

	//character's game objects
	GameObject playerGO;
	GameObject johnGO;

	//buttons
	public GameObject attackButton;
	public GameObject healButton;
	public GameObject okButton;
	public GameObject pumpButton;

	//places where the characters are anchored to
	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	//animators
	animControl anim;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;
	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleStateOK state;

	public int turnNum;

	bool johnIn;
	bool vincentIn;

	bool enemyIsDead;
	bool playerIsDead;

    // Start is called before the first frame update
    void Start()
    {
		state = BattleStateOK.START;
		SetupBattle();
    }

    #region Battle Handler

    void SetupBattle()
	{
		//sets up John and Vincent
		disableJohn();
		johnIn = false;
		DisableVincent();
		vincentIn = false;

		turnNum = 1;

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
		DisableButtons();

		enemyIsDead = enemyUnit.TakeDamage(playerUnit.damage);

		enemyHUD.SetHP(enemyUnit.currentHP);

		dialogueText.text = "Big Oof";

		EnableOkButton();

		//wait for Ok button
	}

	void EnemyTurn()
	{
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
		DisableOkButton();

		state = BattleStateOK.PLAYER_TURN;

		EnableButtons();

		dialogueText.text = "Choose an action:";

		//waits for an action
	}

	void PlayerHeal()
	{
		DisableButtons();

		playerUnit.Heal();

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "Holly shit, that was strong!";

		EnableOkButton();

		//wait for Ok button
	}

	void PlayerPump()
    {
		DisableButtons();

		playerUnit.Pump();

		dialogueText.text = "I mean, butterflies and whatnot, you Know? No you FEEL HYPER!!!";

		EnableOkButton();
    }

    #endregion

    #region buttons

    public void OnAttackButton()
	{
		SetIsAttacking(true);
		PlayerAttack();
	}

	public void OnHealButton()
	{
		SetIsAttacking(true);
		PlayerHeal();
	}

	public void OnPumpButton()
    {
		SetIsAttacking(true);
		PlayerPump();
    }

	public void OnOkButton()
	{
		//for the start
		if (state == BattleStateOK.START)
		{
			PlayerTurn();

			EnableButtons();

			return;
		}

		//for the player attack
		if (state == BattleStateOK.PLAYER_TURN)
		{
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
		if (state == BattleStateOK.ENEMY_TURN)
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
				else if (turnNum == 4)
                {
					EnableVincent();
                }
				else
				{
					state = BattleStateOK.PLAYER_TURN;
					PlayerTurn();
				}
			}

			return;
		}

		//for messages
		if (state == BattleStateOK.MESSAGE)
		{
			state = BattleStateOK.PLAYER_TURN;
			PlayerTurn();

			return;
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

	void DisableButtons()
	{
		attackButton.SetActive(false);
		healButton.SetActive(false);
		pumpButton.SetActive(false);
	}

	void EnableButtons()
	{
		attackButton.SetActive(true);

		if (johnIn)
		{
			healButton.SetActive(true);
		}

		if (vincentIn)
		{
			pumpButton.SetActive(true);
		}

	}

	#endregion

	//sets the value of isAttacking
	void SetIsAttacking(bool value)
    {
		//sets the animator to the prfabs animator
		anim = playerGO.GetComponent<animControl>();    //gets the animator from the player prefab

		//method in the animControl script
		anim.SetIsAttacking(value);
    }

	void disableJohn()
    {
		johnPrefab.SetActive(false);
	}

    void EnableJohn()
    {
		johnPrefab.SetActive(true);

		johnIn = true;

		SetMessage("John Alexander Warnock, aka \"My Human\", joins the battle! ");

		//wait for Ok button
	}

	void DisableVincent()
    {
		vincentPrefab.SetActive(false);
    }

	void EnableVincent()
    {
		vincentPrefab.SetActive(true);

		vincentIn = true;

		SetMessage("Vincent, NINGUEM VAI ENTENDER ISSO, joins the battle!");

		//wait for Ok button
    }

	void SetMessage(string message)
    {
		state = BattleStateOK.MESSAGE;

		dialogueText.text = message;
    }

}
