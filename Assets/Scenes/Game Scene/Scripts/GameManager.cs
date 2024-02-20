using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance;

    private const float k_MonsterDelay = 10f;
    private const float k_MonsterDelayRandomness = 2f;

    private readonly Vector3[] k_progressPositions = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0)
    };

    [SerializeField]
    private Animator m_houseAnimator;

    [SerializeField]
    private TextMeshProUGUI m_timer;

    [SerializeField]
    private Egg m_egg;

    [SerializeField]
    private Transform m_doorTransform;

    [SerializeField]
    private Transform m_monster;

    private Coroutine m_timerRoutine;
    private Coroutine m_monsterCoroutine;

    private int m_remainingTime = 240;
    private bool m_monsterSpawned = false;

    private void Awake()
    {
        s_Instance = this;
        Cursor.visible = false;
        Globals.s_GameOverMessage = "";
        Globals.s_GameOverDescription = "";
        Globals.s_GameOverWin = false;
    }

    public void FinishAnimation()
    {
    }

    private IEnumerator HandleMonster(int progress = 0)
    {
        if (progress < 0)
        {
            yield return new WaitForSeconds(Random.Range(k_MonsterDelay, k_MonsterDelay + k_MonsterDelayRandomness));

            m_monster.localPosition = k_progressPositions[progress];
            yield return HandleMonster(progress + 1);
        }
        else
        {
            GoToGameOver(GameOverReason.MONSTER);
            yield break;
        }
    }

    private IEnumerator TickTime()
    {
        yield return new WaitForSeconds(1);
        m_remainingTime--;
        m_timer.text = (m_remainingTime / 60) + ":";
        if (m_remainingTime % 60 < 10)
        {
            m_timer.text += "0";
        }
        m_timer.text += m_remainingTime % 60;

        if (m_remainingTime > 0)
        {
            /*if (m_remainingTime == 238)
            {
                m_houseAnimator.SetTrigger("monster_entrance");
            }

            if (m_remainingTime < 238)
            {
                float roll = Random.Range(0f, 1f);
                if (roll < 0.30f && !m_monsterSpawned)
                {
                    m_houseAnimator.SetTrigger("monster_spawn");
                    m_monsterCoroutine = StartCoroutine(HandleMonster());
                    m_monsterSpawned = true;
                }
            }
            */
            yield return TickTime();
        }
        else
        {
            m_timerRoutine = null;
            GoToWin();
            yield break;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_timerRoutine = StartCoroutine(TickTime());
        m_egg.StartHatching();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void GoToGameOver(GameOverReason reason)
    {
        Globals.s_GameOverMessage = "You lose.";
        Globals.s_GameOverDescription = GameOverReasonUtility.GetDescription(reason);
        Globals.s_GameOverSurvive = false;

        SceneManager.LoadScene("Game Over Scene");
    }

    public void GoToWin()
    {
        Globals.s_GameOverMessage = "You survied.";
        Globals.s_GameOverDescription = "The egg is healthy...";
        Globals.s_GameOverSurvive = true;

        SceneManager.LoadScene("Game Over Scene");
    }

    public void Egged()
    {
        Globals.s_GameOverMessage = "You are the winner!!!!!! :D";
        Globals.s_GameOverDescription = "This was truly the egg of all time";
        Globals.s_GameOverWin = true;

        SceneManager.LoadScene("Game Over Scene");
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}