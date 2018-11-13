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
        Recovering
    }

    public enum BenderTypes
    {
        Air,
        Earth,
        Fire,
        Water
    }

    public BenderTypes BenderType;
    //[Range(1,2)]
    //public int OwnerPlayerNumber = 1;

    public string Name;
    public States State = States.Idle;

    public float StandardSpeed = 5f;
    public float StandardRotationSpeed = 120f;
    public int AbilityNumber = 3;

    public List<ParticleSystem> AbilitiesPS;

    public TextMeshPro Nameplate;
    public GameObject MeshChild;
    public Material OutlinedMaterial;
    public GameObject Base;


    public List<AudioClip> Footsteps;
    public Sprite PortraitSprite;

    [HideInInspector]
    public Animator BenderAnimator;
    [HideInInspector]
    public float Speed = 0f;
    [HideInInspector]
    public float RotationSpeed = 0f;

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

    protected void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        BenderAnimator = GetComponent<Animator>();
        Nameplate.text = Name;
        _renderer = MeshChild.GetComponent<Renderer>();
        _defaultMaterial = _renderer.material;
        _audioSource = GetComponent<AudioSource>();
        foreach (ParticleSystem ps in AbilitiesPS)
        {
            ps.Stop();
        }
    }

    protected void Start()
    {
        _rotation = transform.rotation.eulerAngles.y;
    }


    protected void Update()
    {
        if (Owner.Type == PlayerController.PlayerTypes.Human)
        {
            if (_selected)
            {

                if (State != States.Idle && State != States.Moving)
                {
                    return;
                }

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
            _rigidbody.rotation = Quaternion.Euler(new Vector3(0f, _rotation + RotationSpeed * Time.deltaTime, 0f));
            _rotation = _rotation + RotationSpeed * Time.deltaTime;
            if (_rigidbody.velocity.magnitude <= 0.01)
            {
                _rigidbody.velocity = new Vector3();
            }
            _rigidbody.velocity += transform.forward * Speed;
        }

        if ((Owner.Type == PlayerController.PlayerTypes.AI && _rigidbody.velocity.magnitude > 0.01) ||
            (Owner.Type == PlayerController.PlayerTypes.Human && NavAgent.velocity.magnitude > 0.01))
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
        BenderAnimator.SetBool("Ability", true);
        State = state;
        AbilitiesPS[number].Play();
        _rigidbody.velocity = new Vector3();
    }

    protected abstract void Cast();

    /*public void ApplyForce(Vector3 force)
    {
        Debug.Log("Applying air force: " + force);
        _rigidbody.AddForce(force);
    }*/

    public void GotHit()
    {
        BenderAnimator.SetTrigger("Hit");
        State = States.Recovering;
        BenderAnimator.SetBool("Moving", false);
    }

    public void GotRevived()
    {
        BenderAnimator.SetTrigger("Revive");
    }

    public void Recovered()
    {
        State = States.Idle;
        NavAgent.enabled = true;
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
        State = States.Idle;
        BenderAnimator.SetBool("Ability", false);
    }

    public void FootstepSound()
    {
        if (Footsteps.Count > 0)
        {
            _audioSource.PlayOneShot(Footsteps[Random.Range(0, Footsteps.Count)]);
        }
    }

    public void Defeat()
    {
        IconObject.RemovedMask.SetActive(true);
        if (Owner.Type == PlayerController.PlayerTypes.Human)
        {
            HumanController hc = Owner.GetComponent<HumanController>();
            if (hc.Selection == this)
            {
                hc.Selection = null;
            }
        }
        Destroy(gameObject);
    }
}
