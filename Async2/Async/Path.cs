using System;
using System.Collections;
using System.Collections.Generic;

namespace Async {

	public enum Status
	{
		OK,
		Continue,
		Now,
		Error
	}

	public delegate Status DAct();
	public delegate Status DPath(Path p);

	/// <summary>
	/// Путь это состояние потока
	/// </summary>
	public class Path {

		List<Delegate> path;
		List<Delegate> error;

		object state = null;

		public Object State {
			get { return state; }
			set { state = value; }
		}

		public T GetState<T>() {return (T)state;}

		public Path Add(DAct act){ this.path.Add(act); return this;}
		public Path Add(DPath pth){ this.path.Add(pth); return this;}
		public Path Error(DAct act){ this.error.Add(act); return this;}
		public Path Error(DPath pth){ this.error.Add(pth); return this; }

		public Path Clear(){ path.Clear(); error.Clear(); return this;}

		public int Count { get { return path.Count; } }
		public bool IsEmpty { get { return this.Count == 0; } }

		public Path Next()
		{
			if (path.Count > 0) path.RemoveAt(0);
			return this;
		}

		public Path Branch(Path branch){
			return this.Add(() => {
				branch.Update();
				if (branch.Count > 0)
					return Status.Continue;
				return Status.Now;
			});
		}
			
		public Path CopyActions(Path p){
			foreach(Delegate d in p.path) {
				if (d.GetType() == typeof(DAct))
					this.Add((DAct)d);
				else
					this.Add((DPath)d);
			}
			return this;
		}


		public void Update(){
			if (Count == 0)
				return;

			Status st = path[0].GetType() == typeof(DAct) ? ((DAct)path[0]).Invoke() : ((DPath)path[0]).Invoke(this);
			switch(st) {
			case Status.OK:
				this.Next();
				break;
			case Status.Now:
				this.Next();
				this.Update();
				break;
			case Status.Error:
				this.path.Clear();
				this.path = this.error;
				this.error = new List<Delegate>();
				break;
			default:
				break;
			}
		}

		public static Path Create() {
			return new Path();
		}

		public static Path Create(Delegate[] dels) {
			Path p = new Path();
			foreach(Delegate d in dels) {
				if (d.GetType() == typeof(DAct))
					p.Add((DAct)d);
				else
					p.Add((DPath)d);
			}
			return p;
		}

		public Path() { 
			path = new List<Delegate>(); 
			error = new List<Delegate>();
		}
	}
}

