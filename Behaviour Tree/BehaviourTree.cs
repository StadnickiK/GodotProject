using Godot;
using System;
using System.Threading.Tasks;

public abstract class BehaviourTree
{
	TreeNode _root = null;

	protected void Start(){
		_root = SetupTree();
	}

	private void Update(){  // probably to change
		if(_root != null)
			_root.Evaluate();
	}

	private async Task UpdateAsync(){  // probably to change
		if(_root != null)
			await _root.EvaluateAsync();
	}

	public TreeNode Root => _root;
	protected abstract TreeNode SetupTree();
}
