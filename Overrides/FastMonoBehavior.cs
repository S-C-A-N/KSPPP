using System;
using UnityEngine;

namespace KSPPP.Overrides
{
	public abstract class FastMonoBehavior : MonoBehaviour
	{
		[NonSerialized] Animation _animation;   
		public new Animation animation { get { return _animation ? _animation : (_animation = GetComponent<Animation>()); } }
		[NonSerialized] AudioSource _audio;   
		public new AudioSource audio { get { return _audio ? _audio : (_audio = GetComponent<AudioSource>()); } }
		[NonSerialized] Camera _camera;   
		public new Camera camera { get { return _camera ? _camera : (_camera = GetComponent<Camera>()); } }
		[NonSerialized] Collider _collider;   
		public new Collider collider { get { return _collider ? _collider : (_collider = GetComponent<Collider>()); } }
		[NonSerialized] Collider2D _collider2D;   
		public new Collider2D collider2D { get { return _collider2D ? _collider2D : (_collider2D = GetComponent<Collider2D>()); } }
		[NonSerialized] ConstantForce _constantForce;   
		public new ConstantForce constantForce { get { return _constantForce ? _constantForce : (_constantForce = GetComponent<ConstantForce>()); } }
		[NonSerialized] GUIText _guiText;   
		public new GUIText guiText { get { return _guiText ? _guiText : (_guiText = GetComponent<GUIText>()); } }
		[NonSerialized] GUITexture _guiTexture;   
		public new GUITexture guiTexture { get { return _guiTexture ? _guiTexture : (_guiTexture = GetComponent<GUITexture>()); } }
		[NonSerialized] HingeJoint _hingeJoint;   
		public new HingeJoint hingeJoint { get { return _hingeJoint ? _hingeJoint : (_hingeJoint = GetComponent<HingeJoint>()); } }
		[NonSerialized] Light _light;   
		public new Light light { get { return _light ? _light : (_light = GetComponent<Light>()); } }
		[NonSerialized] NetworkView _networkView;   
		public new NetworkView networkView { get { return _networkView ? _networkView : (_networkView = GetComponent<NetworkView>()); } }
		[NonSerialized] ParticleEmitter _particleEmitter;   
		public new ParticleEmitter particleEmitter { get { return _particleEmitter ? _particleEmitter : (_particleEmitter = GetComponent<ParticleEmitter>()); } }
		[NonSerialized] ParticleSystem _particleSystem;   
		public new ParticleSystem particleSystem { get { return _particleSystem ? _particleSystem : (_particleSystem = GetComponent<ParticleSystem>()); } }
		[NonSerialized] Renderer _renderer;   
		public new Renderer renderer { get { return _renderer ? _renderer : (_renderer = GetComponent<Renderer>()); } }
		[NonSerialized] Rigidbody _rigidbody;   
		public new Rigidbody rigidbody { get { return _rigidbody ? _rigidbody : (_rigidbody = GetComponent<Rigidbody>()); } }
		[NonSerialized] Rigidbody2D _rigidbody2D;   
		public new Rigidbody2D rigidbody2D { get { return _rigidbody2D ? _rigidbody2D : (_rigidbody2D = GetComponent<Rigidbody2D>()); } }
		[NonSerialized] Transform _transform;   
		public new Transform transform { get { return _transform ? _transform : (_transform = GetComponent<Transform>()); } }
	}
}

