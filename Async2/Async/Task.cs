using System;
using System.Collections;
using System.Collections.Generic;

namespace Async {

	public delegate Status DTsk(Task taks);

	public class Task {
		private DateTime lastUpdate = DateTime.Now;
		private Delegate del { get; set; }
		public int TTL { get { return 60000; } } 
		public string Name { get; set; }

		public float waitBefore = 0.0f;
		public float waitAfter = 0.0f;
		public int count = 1;

		public void Stop(){

		}

		public void Start(){
		}

		public void Update(){
			float delta = (float)DateTime.Now.Subtract(lastUpdate).TotalSeconds;
			lastUpdate = DateTime.Now;
			Update(delta);
		}

		public void Update(float delta)
		{
			Status s = del.GetType() == typeof(DTsk) ? ((DTsk)del).Invoke(this) : ((DAct)del).Invoke();
			switch(s) {
			case Status.Continue:
				return;
			case Status.Error:
			case Status.OK:
			case Status.Now:

				break;
			}
		}
	}
}

