using UnityEngine;
using UnityEngine.Events;

namespace Boss
{
	public class BossStat : MonoBehaviour
	{
		public const int defaultHP = 70;

		public int HP
		{
			get => _hp;
			private set
			{
				_hp = value;
				hpChanged?.Invoke(_hp);
			}
		}

		private int _hp;

		public bool IsDie => HP <= 0;

		public UnityEvent<int> hpChanged;

		private void Awake()
		{
			HP = defaultHP;
		}

		private void OnHit(int damage)
		{
			HP -= damage;
		}
	}
}