using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LitJson;
using System;

public class Recording
{
	public int frameRate;
	public int totalFrames{ get { return frames.Count; } }
	public float recordingLength{ get { return totalFrames / frameRate; } }

	public List<RecordingFrame> frames = new List<RecordingFrame>();

	[System.Serializable]
	public class RecordingFrame
	{
		public List<InputInfo> inputs								= new List<InputInfo>();
		public Dictionary<string,FrameProperty> syncedProperties	= new Dictionary<string, FrameProperty>();
	}
	[System.Serializable]
	public class InputInfo  // represents state of certain input in one frame. Has to be class for inspector to serialize
	{
						  public InputType	inputType;
						  public string 	inputName;				// from InputManager
		[HideInInspector] public int 		mouseButtonNum = -1;	// only positive if is mouse button
		[HideInInspector] public bool 		buttonState;
		[HideInInspector] public float 		axisValue;				// not raw value

		public enum InputType
		{
			Axis,
			Button,
			Key,
			Mouse
		}
		public InputInfo()
		{
			inputName 		= "";
			mouseButtonNum 	= -1;
			inputType 		= InputType.Button;
			buttonState 	= false;
			axisValue 		= 0f;
		}
		public InputInfo( InputInfo from )
		{
			inputName 		= from.inputName;
			inputType 		= from.inputType;
			mouseButtonNum 	= from.mouseButtonNum;
			buttonState 	= from.buttonState;
			axisValue 		= from.axisValue;
		}
		public override bool Equals (object obj)
		{
			return Equals ( obj as InputInfo );
		}
		public bool Equals( InputInfo other )
		{
			if (other == null) 
			{ throw new System.ArgumentNullException ("Recording.Equals(other = null)"); }	// was originally return false
			if ( inputName != other.inputName || inputType != other.inputType || mouseButtonNum != other.mouseButtonNum )
				return false;
			if ( inputType == InputType.Axis )
				return Mathf.Approximately(axisValue,other.axisValue);
			else
				return buttonState == other.buttonState;
		}
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
	}
	[System.Serializable]
	public class FrameProperty
	{
		public string name;
		public string property;
		public FrameProperty()
		{
			// for Json reader...
		}
		public FrameProperty( string name, string property )
		{
			this.name		= name;
			this.property	= property;
		}
	}

	public static Func<int,int,bool> ValidFrame = (cur,max) => ( cur >= 0 && cur <= max ); // originally: if ( atFrame < 0 || atFrame >= frames.Count )

	public Recording()					: this(null,60) {}
	public Recording( int frameRate )	: this(null,frameRate) {}
	public Recording( Recording other)	: this(other,60) {}
	public Recording( Recording other, int frameRate )
	{
		if ( other != null )
		{
			frameRate	= other.frameRate;
			frames 		= new List<RecordingFrame>( other.frames );
		}
		else
		{
			this.frameRate 	= Mathf.Max( 1, frameRate );
			frames 			= new List<RecordingFrame>();
		}
	}

	public static Recording ParseRecording( string jsonRecording )
	{
		ImporterFunc<double, float> importer = delegate( double input) {
			return (float)input;
		};
		JsonMapper.RegisterImporter<double, float>( importer );	// let float values be parsed
		Recording rec = JsonMapper.ToObject<Recording>( jsonRecording );
		JsonMapper.UnregisterImporters();
		return rec;
	}
	public int GetClosestFrame( float toTime )
	{
		return (int)( toTime * frameRate );
	}

	public void			AddInput(	int atFrame, InputInfo inputInfo )
	{
		CheckFrame( atFrame );
		for ( int i = 0; i < frames[atFrame].inputs.Count; i++ )
		{
			// no duplicate properties
			if ( frames[atFrame].inputs[i].inputName == inputInfo.inputName )
			{
				frames[atFrame].inputs[i] = new InputInfo( inputInfo );
				return;
			}
		}
		frames[atFrame].inputs.Add( new InputInfo( inputInfo ) );
	}
	public InputInfo	GetInput(	int atFrame, string inputName )
	{
		if(ValidFrame(atFrame,frames.Count))
		{
			// iterating to find. Could avoid repeat access time with pre-processing, but would be a waste of memory/GC slowdown? & list is small anyway
			foreach ( InputInfo input in frames[atFrame].inputs )
				if ( input.inputName == inputName )
					return input;
		}
		else
		{
			Debug.LogWarning( "Frame " + atFrame + " out of bounds" );
			return null;
		}

		Debug.LogWarning( "Input " + inputName + " not found in frame " + atFrame );
		return null;
	}
	public InputInfo[]	GetInputs(	int atFrame )
	{
		if (ValidFrame(atFrame,frames.Count))
			return frames[atFrame].inputs.ToArray();
		else
		{
			Debug.LogWarning( "Frame " + atFrame + " out of bounds" );
			return new InputInfo[0];
		}
	}

	public void AddProperty( int atFrame, string propertyName, string propertyValue )
	{
		CheckFrame( atFrame );
		FrameProperty existingProp;
		if ( frames[atFrame].syncedProperties.TryGetValue( propertyName, out existingProp ) )
			existingProp.property = propertyValue;
		else
			frames[atFrame].syncedProperties.Add( propertyName, new FrameProperty( propertyName, propertyValue ) );
	}
	public string GetProperty( int atFrame, string propertyName )
	{
		if (ValidFrame(atFrame,frames.Count))
		{
			// iterating to find. Could avoid repeat access time with pre-processing, but would be a waste of memory/GC slowdown? & list is small anyway
			FrameProperty frameProp;
			if ( frames[atFrame].syncedProperties.TryGetValue( propertyName, out frameProp ) )
				return frameProp.property;
		}
		else
		{
			Debug.LogWarning( "Frame " + atFrame + " out of bounds" );
			return null;
		}

		return null;
	}

	void CheckFrame( int frame )
	{
		while ( frame >= frames.Count )
			frames.Add( new RecordingFrame() );
	}
	public override string ToString()
	{
		StringBuilder jsonB = new StringBuilder();
		JsonWriter writer = new JsonWriter( jsonB );
		writer.WriteObjectStart();
		//{
		writer.WritePropertyName( "frameRate" );
		writer.Write( frameRate );
		writer.WritePropertyName( "frames" );
		writer.WriteArrayStart();
		//[
		foreach ( RecordingFrame frame in frames )
		{
			writer.WriteObjectStart();
			//{
			writer.WritePropertyName( "inputs" );
			writer.WriteArrayStart();
			//[
			foreach ( InputInfo input in frame.inputs )
			{
				writer.WriteObjectStart();
				//{
				writer.WritePropertyName( "inputName" );
				writer.Write( input.inputName );
				writer.WritePropertyName( "inputType" );
				writer.Write( (int)input.inputType );
				writer.WritePropertyName( "mouseButtonNum" );
				writer.Write( input.mouseButtonNum );
				writer.WritePropertyName( "buttonState" );
				writer.Write( input.buttonState );
				writer.WritePropertyName( "axisValue" );
				writer.Write( input.axisValue );
				//}
				writer.WriteObjectEnd();
			}
			//]
			writer.WriteArrayEnd();
			writer.WritePropertyName( "syncedProperties" );
			writer.WriteObjectStart();
			//[
			foreach ( var prop in frame.syncedProperties )
			{
				writer.WritePropertyName( prop.Key );
				writer.WriteObjectStart();
				//{
				writer.WritePropertyName( "name" );
				writer.Write( prop.Value.name );
				writer.WritePropertyName( "property" );
				writer.Write( prop.Value.property );
				//}
				writer.WriteObjectEnd();
			}
			//]
			writer.WriteObjectEnd();
			//}
			writer.WriteObjectEnd();
		}
		//]
		writer.WriteArrayEnd();
		//}
		writer.WriteObjectEnd();
		return jsonB.ToString();
	}

	public static void Main() {
		var a = new Recording();
		var b = new Recording(60);


	}
}