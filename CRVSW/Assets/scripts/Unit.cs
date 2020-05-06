using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

	public string unitName;
	//public int unitLevel;

	public int startingDamage;
	public int damage;

	public int startingMultiplier;
	public int multiplier;

	public int startingHeal;
	public int healAmount;

	public int maxHP;
	public int currentHP;

	public bool TakeDamage(int dmg)
	{
		currentHP -= dmg;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Heal()
	{
		currentHP += healAmount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

	public void Pump()
    {
		multiplier += 5;
		healAmount += multiplier;
		damage += multiplier;
    }

	public void Reset()
    {
		damage = startingDamage;
		multiplier = startingMultiplier;
		currentHP = maxHP;
    }

}
