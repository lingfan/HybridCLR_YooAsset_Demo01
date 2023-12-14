
using UniFramework.Machine;

internal class FsmInitInitHotUpdate : IStateNode
{
	private StateMachine _machine;

	void IStateNode.OnCreate(StateMachine machine)
	{
		_machine = machine;
	}
	void IStateNode.OnEnter()
	{
		Prepare();
	}
	void IStateNode.OnUpdate()
	{
	}
	void IStateNode.OnExit()
	{
	}

	private void Prepare()
	{
		
		// 初始化窗口系统
		//UniWindow.Initalize(desktop);

		// 初始化对象池系统
		// UniPooling.Initalize();

		_machine.ChangeState<FsmLunchHotUpdate>();
	}
}