using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : InteractableComponent
{
    [SerializeField]
    private LayerMask m_highlightMask;

    [SerializeField]
    private Transform m_rightHandTransform;

    [SerializeField]
    private Animator m_eyeLidAnimator;

    [SerializeField]
    private ParticleSystem m_bloodParticles;

    [SerializeField]
    private TextMeshProUGUI m_bloodText;

    [SerializeField]
    private TextMeshProUGUI m_insanityText;

    private int m_interactableLayer = -1;
    private int m_uiLayer = -1;

    private Camera m_mainCamera;
    private Animator m_cameraAnimator;

    private Coroutine m_healRoutine;

    private bool m_isBleeding = false;
    private bool m_calming = false;
    private bool m_blackedOut = false;
    private bool m_inAnimation = false;

    private int m_previousColliderId = -1;
    private int m_previousLayer = -1;
    private Renderer m_previousRenderer = null;
    private Transform m_previousParent = null;
    private Transform m_totalTransform = null;
    private Vector3 m_previousPosition = Vector3.zero;
    private Quaternion m_previousRotation = Quaternion.identity;

    private const int k_BlackoutMin = 2;
    private const int k_BlackoutMax = 4;
    private const int k_BloodLimit = 120;
    private const int k_BloodRegenRate = 12;
    private const float k_BloodRegenDelay = 5f;

    private const int k_InsanityRate = 4;
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
        if (effect.repeat == 0)
        {
            RemoveEffect(EffectType.BLEED);
            yield break;
        }

        yield return new WaitForSeconds(effect.delay);
        m_blood = Math.Max(m_blackoutLimit, m_blood - effect.magnitude);
        if (m_blood <= m_blackoutLimit)
        {
            m_blackedOut = true;
            StartCoroutine(HandleBlackout());
            RemoveEffect(EffectType.BLEED);
            yield break;
        }

        effect.repeat--;
        yield return HandleBleed(effect);
    }

    private IEnumerator HandleInsanity()
    {
        yield return new WaitForSeconds(k_InsanityDelay);

        if (!m_calming) m_insanity += k_InsanityRate;
        if (m_insanity > k_InsanityLimit)
        {
            GameManager.s_Instance.GoToGameOver(GameOverReason.INSANITY);
        }

        yield return HandleInsanity();
    }

    private IEnumerator HandleBlackout()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(k_BlackoutMin, k_BlackoutMax));

        m_blackedOut = false;

        yield break;
    }

    private IEnumerator HandleHeal()
    {
        yield return new WaitForSeconds(k_BloodRegenDelay);

        m_blood = Math.Min(m_blood + k_BloodRegenRate, k_BloodLimit);

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

    public void EndAnimation()
    {
        m_inAnimation = false;
        if (m_heldItem.Type == ItemType.FRYING_PAN)
        {
            GameManager.s_Instance.Egged();
        }
    }

    public override void Interact(InteractableComponent sender)
    {
    }

    public override bool AcceptItem(ItemComponent item)
    {
        return false;
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
                m_bloodParticles.Play();
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
                    m_bloodParticles.Stop();
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

    private void Awake()
    {
        m_uiLayer = LayerMask.NameToLayer("UI");
        m_interactableLayer = LayerMask.NameToLayer("Interactable");
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_effects = new Dictionary<EffectType, Tuple<Effect, Coroutine>>();
        m_mainCamera = Camera.main;
        m_cameraAnimator = m_mainCamera.GetComponent<Animator>();
        StartCoroutine(HandleInsanity());
        m_healRoutine = StartCoroutine(HandleHeal());
        m_bloodParticles.Stop();
    }

    // Update is called once per frame
    private void Update()
    {
        m_insanityText.text = m_insanity.ToString();
        m_bloodText.text = m_blood.ToString();

        if (m_inAnimation) return;
        if (m_blackedOut) return;
        if (m_calming && Input.GetKey(KeyCode.H)) return;

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

                if (m_heldItem == null || m_heldItem.IsValidTarget(m_focusedInteractable) ||
                    m_focusedInteractable.WillAcceptItem(m_heldItem))
                {
                    if (m_previousRenderer.materials.Length > 1)
                        m_previousRenderer.materials[1].SetFloat("_Outline_Thickness", 0.01f);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (m_heldItem != null)
                {
                    if (m_focusedInteractable.Type == InteractableType.ENTITY)
                    {
                        if (!m_focusedInteractable.AcceptItem(m_heldItem))
                            m_heldItem.Use(m_focusedInteractable);

                        if (m_heldItem.GetUseAnimationTrigger() != null)
                        {
                            m_inAnimation = true;
                            m_cameraAnimator.Rebind();
                            m_cameraAnimator.SetTrigger(m_heldItem.GetUseAnimationTrigger());
                        }
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

                    if (m_focusedInteractable.Type == InteractableType.ITEM)
                    {
                        ItemComponent item = (ItemComponent)m_focusedInteractable.GetCachedComponent();
                        if (!item.ForceOnSelf() || item.IsValidTarget(this))
                        {
                            m_heldItem = item;
                            m_totalTransform = item.transform;
                            while (m_totalTransform.parent != null &&
                                m_totalTransform.parent.gameObject.layer == m_interactableLayer)
                            {
                                m_totalTransform = m_totalTransform.parent;
                            }

                            m_previousLayer = m_heldItem.gameObject.layer;
                            m_previousPosition = m_totalTransform.localPosition;
                            m_previousRotation = m_totalTransform.localRotation;
                            m_previousParent = m_totalTransform.parent;

                            m_heldItem.gameObject.layer = m_uiLayer;
                            m_totalTransform.parent = m_rightHandTransform;
                            m_totalTransform.localPosition = m_heldItem.GetHoldPosition();
                            m_totalTransform.localRotation = m_heldItem.GetHoldRotation();
                            m_focusedInteractable = null;
                            if (item.ForceOnSelf() && item.IsValidTarget(this))
                            {
                                item.Use(this);
                                if (item.GetUseAnimationTrigger() != null)
                                {
                                    m_inAnimation = true;
                                    m_cameraAnimator.Rebind();
                                    m_cameraAnimator.SetTrigger(item.GetUseAnimationTrigger());
                                }

                                if (m_previousRenderer.materials.Length > 1)
                                    m_previousRenderer.materials[1].SetFloat("_Outline_Thickness", 0f);
                            }
                        }
                    }
                    else
                    {
                        m_focusedInteractable.Interact(this);
                    }
                }
            }
        }
        else
        {
            if (m_previousColliderId != -1)
            {
                if (m_previousRenderer.materials.Length > 1)
                    m_previousRenderer.materials[1].SetFloat("_Outline_Thickness", 0f);

                m_previousColliderId = -1;
                m_focusedInteractable = null;
                m_previousRenderer = null;
            }
        }

        if (Input.GetMouseButtonUp(0) && m_focusedInteractable != null)
        {
            if (m_isBleeding)
            {
                m_focusedInteractable.RemoveEffect(EffectType.BLOOD_FED);
            }
        }

        if (Input.GetKey(KeyCode.H))
        {
            if (!m_calming)
            {
                ApplyEffect(new Effect(EffectType.CALM,
                                    k_InsanityRate,
                                    k_InsanityDelay / 2,
                                    -1));

                m_eyeLidAnimator.SetTrigger("close");
                SoundManager.s_Instance.PlaySlowBreath();
            }
        }
        else
        {
            if (m_calming)
            {
                RemoveEffect(EffectType.CALM);
                m_eyeLidAnimator.SetTrigger("open");
                SoundManager.s_Instance.StopSlowBreath();
            }
        }

        if (Input.GetKeyDown(KeyCode.C) && m_heldItem != null)
        {
            switch (m_heldItem.Type)
            {
                case ItemType.BLOOD_VIAL:
                    SoundManager.s_Instance.PlayVialPutSound();
                    break;

                case ItemType.KNIFE:
                    SoundManager.s_Instance.PlayKnifePutSound();
                    break;

                default:
                    break;
            }

            m_heldItem.gameObject.layer = m_previousLayer;
            m_totalTransform.parent = m_previousParent;
            m_cameraAnimator.Rebind();
            m_totalTransform.localPosition = m_previousPosition;
            m_totalTransform.localRotation = m_previousRotation;
            m_heldItem = null;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public override bool WillAcceptItem(ItemComponent item)
    {
        return false;
    }
}