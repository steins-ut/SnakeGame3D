using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : InteractableComponent
{
    [SerializeField]
    private LayerMask m_highlightMask;

    [SerializeField]
    private Transform m_rightHandTransform;

    private int m_uiLayer = -1;

    private Camera m_mainCamera;

    private Coroutine m_healRoutine;

    private bool m_isBleeding = false;
    private bool m_calming = false;
    private bool m_blackingOut = false;
    private bool m_inAnimation = false;

    private int m_previousColliderId = -1;
    private int m_previousLayer = -1;
    private Renderer m_previousRenderer = null;
    private Transform m_previousParent = null;
    private Vector3 m_previousPosition = Vector3.zero;
    private Quaternion m_previousRotation = Quaternion.identity;

    private const int k_BlackoutMin = 2;
    private const int k_BlackoutMax = 4;
    private const int k_BloodLimit = 120;
    private const int k_BloodRegenRate = 12;
    private const float k_BloodRegenDelay = 5f;

    private const int k_InsanityRate = 2;
    private const float k_InsanityDelay = 1f;
    private const int k_InsanityLimit = 100;

    private int m_blood = k_BloodLimit;

    private int m_blackoutLimit = 12;

    private int m_insanity = 0;

    private ItemComponent m_heldItem = null;
    private InteractableComponent m_focusedInteractable = null;
    private Dictionary<EffectType, Tuple<Effect, Coroutine>> m_effects;

    public override InteractableType Type => InteractableType.ENTITY;

    private IEnumerator HandleBleed(Effect effect)
    {
        if(effect.repeat == 0)
        {
            RemoveEffect(EffectType.BLEED);
            yield break;
        }

        yield return new WaitForSeconds(effect.delay);
        m_blood = Math.Max(m_blackoutLimit, m_blood - effect.magnitude);
        if (m_blood == m_blackoutLimit)
        {
            m_blackingOut = true;
            RemoveEffect(EffectType.BLEED);
            yield break;
        }

        effect.repeat--;
        yield return HandleBleed(effect);
    }

    private IEnumerator HandleInsanity()
    {
        yield return new WaitForSeconds(k_InsanityDelay);

        if(!m_calming) m_insanity += k_InsanityRate;
        if (m_insanity > k_InsanityLimit)
        {
            //die code
            Debug.Log("you dieded");
        }

        yield return HandleInsanity();
    }

    private IEnumerator HandleHeal()
    {
        yield return new WaitForSeconds(k_BloodRegenDelay);

        m_blood = Math.Max(m_blood + k_BloodRegenRate, k_BloodLimit);

        yield return HandleHeal();
    }

    private IEnumerator HandleCalm(Effect effect)
    {
        if (effect.repeat == 0)
        {
            m_effects.Remove(EffectType.CALM);
            yield break;
        }

        yield return new WaitForSeconds(effect.delay);

        m_insanity = Math.Max(0, m_insanity - effect.magnitude);

        if (effect.repeat > 0) { effect.repeat--; }

        yield return HandleCalm(effect);
    }

    private void Awake()
    {
        m_uiLayer = LayerMask.NameToLayer("UI");
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_effects = new Dictionary<EffectType, Tuple<Effect, Coroutine>>();
        m_mainCamera = Camera.main;
        StartCoroutine(HandleInsanity());
        m_healRoutine = StartCoroutine(HandleHeal());
    }

    public override bool AcceptItem(ItemComponent item)
    {
        throw new NotImplementedException();
    }

    public override void ApplyEffect(Effect effect)
    {
        //switch and run handling coroutine
        if (m_effects.ContainsKey(effect.type))
        {
            StopCoroutine(m_effects[effect.type].Item2);
        }

        switch (effect.type)
        {
            case EffectType.BLEED:
                StopCoroutine(m_healRoutine);
                m_effects[effect.type] = Tuple.Create(effect, StartCoroutine(HandleBleed(effect)));
                m_isBleeding = true;
                break;

            case EffectType.CALM:
                m_effects[effect.type] = Tuple.Create(effect, StartCoroutine(HandleCalm(effect)));
                m_calming = true;
                break;

            default:
                break;
        }
    }

    public override void RemoveEffect(EffectType effectType)
    {
        if (m_effects.ContainsKey(effectType))
        {
            StopCoroutine(m_effects[effectType].Item2);
            m_effects.Remove(effectType);
            switch (effectType)
            {
                case EffectType.BLEED:
                    m_healRoutine = StartCoroutine(HandleHeal());
                    m_isBleeding = false;
                    break;

                case EffectType.CALM:
                    m_calming = false;
                    break;

                default:
                    break;
            }
        }
    }

    public bool IsBleeding()
    {
        return m_isBleeding;
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
                if (m_heldItem != null)
                {
                    if (m_focusedInteractable.Type == InteractableType.ENTITY)
                    {
                        m_heldItem.Use(m_focusedInteractable);
                    }
                }
                else
                {
                    if (m_effects.ContainsKey(EffectType.BLEED))
                    {
                        m_focusedInteractable.ApplyEffect(new Effect(
                            EffectType.BLOOD_FED,
                            m_effects[EffectType.BLEED].Item1.magnitude,
                            m_effects[EffectType.BLEED].Item1.delay,
                            -1));
                    }

                    if(m_focusedInteractable.Type == InteractableType.ITEM)
                    {
                        ItemComponent item = (ItemComponent)m_focusedInteractable.GetCachedComponent();
                        if(!item.ForceOnSelf() || item.IsValidTarget(this))
                        {
                            m_heldItem = item;
                            m_previousLayer = m_heldItem.gameObject.layer;
                            m_previousPosition = m_heldItem.transform.localPosition;
                            m_previousRotation = m_heldItem.transform.localRotation;
                            m_previousParent = m_heldItem.transform.parent;

                            m_heldItem.gameObject.layer = m_uiLayer;
                            m_heldItem.transform.parent = m_rightHandTransform;
                            m_heldItem.transform.localPosition = m_heldItem.GetHoldPosition();
                            m_heldItem.transform.localRotation = m_heldItem.GetHoldRotation();
                            m_focusedInteractable = null;
                            if (item.ForceOnSelf() && item.IsValidTarget(this))
                            {
                                item.Use(this);
                            }
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
            if(m_isBleeding)
            {
                m_focusedInteractable.RemoveEffect(EffectType.BLOOD_FED);
            }
        }

        if (Input.GetKey(KeyCode.H))
        {
            if (!m_calming)
                ApplyEffect(new Effect(EffectType.CALM,
                                    k_InsanityRate,
                                    k_InsanityDelay / 2,
                                    -1));
        }
        else
        {
            if (m_calming)
            {
                RemoveEffect(EffectType.CALM);
            }
        }

        if(Input.GetKeyDown(KeyCode.C) && m_heldItem != null)
        {
            m_heldItem.gameObject.layer = m_previousLayer;
            m_heldItem.transform.parent = m_previousParent;
            m_heldItem.transform.localPosition = m_previousPosition;
            m_heldItem.transform.localRotation = m_previousRotation;
            m_heldItem = null;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}