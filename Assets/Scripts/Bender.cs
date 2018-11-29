using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public abstract class Bender : MonoBehaviour
{
    public enum States
    {
        Idle,
        Moving,
        Casting1,
        Casting2,
        Casting3,
        Recovering,
        Frozen
    }

    public enum BenderTypes
    {
        Air,
        Earth,
        Fire,
        Water
    }

    public BenderTypes BenderType;

    public string Name;
    public States State = States.Idle;

    public float StandardSpeed = 5f;
    public float StandardRotationSpeed = 120f;
    public int AbilityNumber = 3;

    public Transform ProjectileSpawnPoint;

    public List<ParticleSystem> AbilitiesPS;

    public TextMeshPro Nameplate;
    public GameObject MeshChild;
    public Material OutlinedMaterial;
    public Material FrozenMaterial;
    public GameObject Base;


    public List<AudioClip> Footsteps;
    public AudioClip FallSound;
    public AudioClip AbilitySound;
    public Sprite PortraitSprite;

    [HideInInspector]
    public Animator BenderAnimator;
    [HideInInspector]
    public float SpeedInput = 0f;
    [HideInInspector]
    public float RotationSpeedInput = 0f;
    [HideInInspector]
    public int AbilitySelector = -1;

    [HideInInspector]
    public NavMeshAgent NavAgent;

    [HideInInspector]
    private PlayerController _owner;

    protected bool _selected = false;

    protected Rigidbody _rigidbody;
    protected Renderer _renderer;
    protected Material _defaultMaterial;
    protected AudioSource _audioSource;
    protected BenderIconController _iconObject;

    protected float _rotation;

    protected float _vulnerability;

    protected Material _prevMaterial;

    public BenderIconController IconObject
    {
        get
        {
            return _iconObject;
        }

        set
        {
            _iconObject = value;
        }
    }

    public PlayerController Owner
    {
        get
        {
            return _owner;
        }

        set
        {
            _owner = value;

            Color baseColor = Owner.PlayerColor;
            baseColor.a = 0.5f;
            Base.GetComponent<Renderer>().material.color = baseColor;
        }
    }

    public AudioSource AudioSourcePlayer
    {
        get
        {
            return _audioSource;
        }
    }

    protected void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        BenderAnimator = GetComponent<Animator>();
        Nameplate.text = Name;
        _renderer = MeshChild.GetComponent<Renderer>();
        _defaultMaterial = _renderer.materials[0];
        _audioSource = GetComponent<AudioSource>();
        foreach (ParticleSystem ps in AbilitiesPS)
        {
            ps.Stop();
        }
    }

    protected void Start()
    {
        _vulnerability = 1.0f;
        _rotation = transform.rotation.eulerAngles.y;
    }


    protected void Update()
    {
        if (State != States.Idle && State != States.Moving)
        {
            return;
        }
        if (Owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            if (_selected)
            {
                if (Input.GetButtonDown("Ability1"))
                {
                    StartAbility(States.Casting1, 0);
                    return;
                }
                else if (Input.GetButtonDown("Ability2"))
                {
                    StartAbility(States.Casting2, 1);
                    return;
                }
                else if (AbilityNumber >= 3 && Input.GetButtonDown("Ability3"))
                {
                    StartAbility(States.Casting3, 2);
                    return;
                }
            }
        }
        else
        {
            switch (AbilitySelector)
            {
                case 0:
                    StartAbility(States.Casting1, 0);
                    return;
                case 1:
                    StartAbility(States.Casting2, 1);
                    return;
                case 2:
                    StartAbility(States.Casting3, 2);
                    return;
                default:
                    break;
            }
            AbilitySelector = -1;
            
            _rotation += RotationSpeedInput * StandardRotationSpeed * Time.deltaTime;
            _rigidbody.rotation = (Quaternion.Euler(new Vector3(0f, _rotation, 0f)));
            if (_rigidbody.velocity.magnitude <= 0.01 && _rigidbody.velocity.y <= Mathf.Epsilon)
            {
                _rigidbody.velocity = new Vector3();
            }
            _rigidbody.velocity = transform.forward * SpeedInput * StandardSpeed;
        }
        
        if (((Owner.Type == PlayerController.PlayerTypes.AI || Owner.Type == PlayerController.PlayerTypes.HumanKeyBoard) && Mathf.Abs(SpeedInput) > 0.01) ||
            ((Owner.Type == PlayerController.PlayerTypes.HumanMouse) && NavAgent.velocity.magnitude > 0.01))
        {
            BenderAnimator.SetBool("Moving", true);
            State = States.Moving;
        }
        else
        {
            BenderAnimator.SetBool("Moving", false);
            State = States.Idle;
        }

    }

    public void StartAbility(States state, int number)
    {
        if (_owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            NavAgent.isStopped = true;
        }
        BenderAnimator.SetBool("Ability", true);
        State = state;
        AbilitiesPS[number].Play();
        _rigidbody.velocity = new Vector3();
        AbilitySelector = -1;
    }

    protected abstract void Cast();

    /*public void ApplyForce(Vector3 force)
    {
        Debug.Log("Applying air force: " + force);
        _rigidbody.AddForce(force);
    }*/

    public void GotHit()
    {
        _vulnerability += 0.05f;
        BenderAnimator.SetTrigger("Hit");
        State = States.Recovering;
        BenderAnimator.SetBool("Moving", false);
        if (_owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            NavAgent.isStopped = true;
        }
    }

    public void GotRevived()
    {
        BenderAnimator.SetTrigger("Revive");
    }

    public void Recovered()
    {
        State = States.Idle;
        if (_owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            NavAgent.isStopped = false;
        }
    }

    public void GotSelected()
    {
        _renderer.material = OutlinedMaterial;
        IconObject.SelectionBox.SetActive(true);
        _selected = true;
    }

    public void GotDeselected()
    {
        _renderer.material = _defaultMaterial;
        IconObject.SelectionBox.SetActive(false);
        _selected = false;
    }

    public void FinishCast()
    {
        _audioSource.PlayOneShot(AbilitySound);
        State = States.Idle;
        BenderAnimator.SetBool("Ability", false);
        if (_owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            NavAgent.isStopped = false;
        }
    }

    public void GotFrozen(float freezeDuration)
    {
        _prevMaterial = _renderer.material;
        _renderer.material = FrozenMaterial;
        BenderAnimator.speed = 0f;
        State = States.Frozen;

        StartCoroutine(Unfreeze(freezeDuration));
    }

    protected IEnumerator Unfreeze(float freezeDuration)
    {
        yield return new WaitForSeconds(freezeDuration);
        if (State == States.Frozen)
        {
            GotUnfrozen();
        }
    }

    public void GotUnfrozen()
    {
        _renderer.material = _prevMaterial;
        BenderAnimator.speed = 1f;
        BenderAnimator.SetTrigger("Reset");
        Recovered();
    }

    public void FootstepSound()
    {
        if (Footsteps.Count > 0)
        {
            _audioSource.PlayOneShot(Footsteps[Random.Range(0, Footsteps.Count)]);
        }
    }

    public void PlayFallSound()
    {
        _audioSource.PlayOneShot(FallSound);
    }

    public void Defeat()
    {
        IconObject.RemovedMask.SetActive(true);
        if (Owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            HumanControllerMouse hc = Owner.GetComponent<HumanControllerMouse>();
            if (hc.Selection == this)
            {
                hc.Selection = null;
            }
        }
        Destroy(gameObject);
    }

    public float getVulnerability()
    {
        return _vulnerability;
    }

    public bool isSelected()
    {
        return _selected;
    }
}
