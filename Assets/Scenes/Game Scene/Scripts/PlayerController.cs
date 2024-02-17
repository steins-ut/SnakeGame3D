using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : InteractableComponent
{
    [SerializeField]
    private LayerMask m_highlightMask;

    private Camera m_mainCamera;

    private Coroutine m_calmRoutine = null;

    private bool m_bloodyHands = false;
    private bool m_calming = false;
    private bool m_blackingOut = false;
    private bool m_inAnimation = false;

    private int m_previousColliderId = -1;
    private Renderer m_previousRenderer = null;

    private const int m_blackoutMin = 2;
    private const int m_blackoutMax = 4;

    private const int k_InsanityRate = 2;
    private const int k_InsanityTime = 1;
    private const int k_InsanityLimit = 100;

    private int m_blood = 120;
    private int m_bloodRegenRate = 12;
    private int m_bleedRate = 8;
    private int m_blackoutLimit = 12;

    private int m_insanity = 0;

    private ItemComponent m_heldItem = null;
    private InteractableComponent m_focusedInteractable = null;
    private Dictionary<EffectType, Tuple<int, Coroutine>> m_effects;

    public override InteractableType Type => throw new NotImplementedException();

    public void TakeEffect(EffectType effect, int level)
    {
        //switch and run handling coroutine
        if (m_effects.ContainsKey(effect))
        {
            StopCoroutine(m_effects[effect].Item2);
        }

        switch (effect)
        {
            case EffectType.BLEED:
                m_effects[effect] = Tuple.Create(level, StartCoroutine(HandleBleed(level)));
                break;

            default:
                Debug.Log("Unsupported effect.");
                break;
        }
    }

    private IEnumerator HandleBleed(int level)
    {
        int bleedCount = 0;
        while (bleedCount < 9)
        {
            yield return new WaitForSeconds(0);
            bleedCount++;
            m_blood = Math.Max(m_blackoutLimit, m_blood - m_bleedRate);
            if(m_blood == m_blackoutLimit)
            {
                m_blackingOut = true;
                m_bloodyHands = false;
                m_effects.Remove(EffectType.BLEED);
                yield break;
            }
        }

        yield break;
    }

    private IEnumerator HandleInsanity()
    {
        Debug.Log(m_insanity);

        yield return new WaitForSeconds(k_InsanityTime);

        m_insanity += k_InsanityRate;
        if (m_insanity > k_InsanityLimit)
        {
            //die code
            Debug.Log("you dieded");
        }

        yield return HandleInsanity();
    }

    private IEnumerator CalmDown()
    {
        yield return new WaitForSeconds(k_InsanityTime / 2);

        if(!m_calming) m_insanity = Math.Max(0, m_insanity - k_InsanityRate);

        yield return CalmDown();
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_effects = new Dictionary<EffectType, Tuple<int, Coroutine>>();
        m_mainCamera = Camera.main;
        StartCoroutine(HandleInsanity());
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_inAnimation) return;

        RaycastHit hit;

        if (Physics.Raycast(m_mainCamera.transform.position, m_mainCamera.transform.forward, out hit, 10f, m_highlightMask))
        {
            if (hit.colliderInstanceID != m_previousColliderId)
            {
                if (m_previousRenderer != null)
                {
                    if (m_previousRenderer.materials.Length > 1)
                        m_previousRenderer.materials[1].SetFloat("_Outline_Thickness", 0f);
                }
                m_focusedInteractable = hit.collider.GetComponent<InteractableComponent>();
                m_previousColliderId = hit.colliderInstanceID;
                m_previousRenderer = hit.collider.GetComponent<Renderer>();

                if (m_heldItem == null || m_heldItem.IsValidTarget(m_focusedInteractable)) { 
                    if (m_previousRenderer.materials.Length > 1)
                        m_previousRenderer.materials[1].SetFloat("_Outline_Thickness", 0.1f);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (m_effects.ContainsKey(EffectType.BLEED))
                {
                    m_focusedInteractable.ApplyEffect(EffectType.BLOOD_FED, m_effects[EffectType.BLEED].Item1);
                    return;
                }

                if (m_heldItem != null)
                {
                    if (m_focusedInteractable.Type == InteractableType.ENTITY)
                    {
                        m_focusedInteractable.AcceptItem(m_heldItem);
                        m_heldItem = null;
                    }
                }
                else
                {
                    if (m_focusedInteractable.Type == InteractableType.ITEM)
                    {
                        ItemComponent item = (ItemComponent)m_focusedInteractable.GetCachedComponent();
                        if (!item.ForceOnSelf(this))
                        {
                            m_heldItem = item;
                        }
                    }
                }
            }
        }
        else
        {
            if(m_previousColliderId != -1)
            {
                if (m_previousRenderer.materials.Length > 1)
                    m_previousRenderer.materials[1].SetFloat("_Outline_Thickness", 0f);

                m_previousColliderId = -1;
                m_focusedInteractable = null;
                m_previousRenderer = null;
            }
        }

        if(Input.GetMouseButtonUp(0) && m_focusedInteractable != null) {
            m_focusedInteractable.RemoveEffect(EffectType.BLOOD_FED);
        }

        if (Input.GetKey(KeyCode.H) && !m_calming)
        {
            m_calmRoutine = StartCoroutine(CalmDown());
        }
        else
        {
            if (m_calming)
            {
                StopCoroutine(m_calmRoutine);
                m_calmRoutine = null;
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public override bool AcceptItem(ItemComponent item)
    {
        throw new NotImplementedException();
    }

    public override void ApplyEffect(EffectType effect, int level)
    {
        throw new NotImplementedException();
    }

    public override void RemoveEffect(EffectType effect)
    {
        throw new NotImplementedException();
    }
}