# Unity 配合 DragonBones 做2D游戏 遇到的问题一 Swordsman 为例子

时间: 2017/8/7

我想做的是一个 2D的 小Demo, 如图
![Snip20170807_1.png](http://upload-images.jianshu.io/upload_images/1476913-d22411bc42e2d2cf.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

但是我遇到了麻烦, 在从 DragonBones 导入 Swordsman 案例之后 游戏人物 的动画出现了问题.

###问题:
刚导入是没问题的
![Snip20170807_2.png](http://upload-images.jianshu.io/upload_images/1476913-7222cf92f05996c5.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

但是给游戏中的 Swordsman 对象 挂上代码之后 运行 就成这样了. 

Unity 中运行的结果 (steady 状态)
![Snip20170807_3.png](http://upload-images.jianshu.io/upload_images/1476913-999127094fbc46e0.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

Unity 中运行结果 (walk 状态)
![Snip20170807_4.png](http://upload-images.jianshu.io/upload_images/1476913-1d2c9bbc7b23b04d.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)


###原因:

是代码问题.
第一: 是没有在 `Start()` 方法里面将初始状态的动画清除掉. 

第二: 我的思路是将 `steady`, `walk`, `jump` 归为一组(我这里只用了这3个位移动画, 没有用到 `steady2`等其他的 位移动画). 然后在代码里面 我在修改一个动画时(比如从 `steady` 状态 变成 `walk` 状态), 没有将之前的 `steady`状态停止并设置为 `null`.


###解决方法:

#####1. 首先是 需要在 `Start()` 方法里面写句代码

```
// 提示: _armatureComponent 是在 Awake() 里面拿到的 UnityArmatureComponent 组件
void Start ()
{
	// 这一步很重要, 如果没有将 初始动画消除的话, 会叠加到 Update方法 的动画里面
	_armatureComponent.animation.Reset ();
}
```

即 在游戏一开始就清除所有的游戏状态, 放在初始状态叠加到后面的游戏状态上, 而我一开始出现的 播放动画导致图片错乱有一部分原因就是这个导致的.

#####2. 在同一组动画中 切换动画时, 记得将同组中的其他动画(DragonBones.AnimationState)关闭

如我这边的 `jump` 动画播放时

```
/**
 * 跳跃
 */ 
private void _jump ()
{

	// 判断是否在地面上
	if (PlayerIsOnGround ()) {

		// 1. 改变状态
		_curState = PlayerState.JUMP;

		// 2. 淡入跳跃动画
		_jumpState = _armatureComponent.animation.FadeIn ("jump", -1.0f, 1, 0, NORMAL_ANIMATION_GROUP, AnimationFadeOutMode.SameGroup);
		_jumpState.timeScale = 2.0f;  // 控制 某个动画状态 的 播放速度
		_jumpState.autoFadeOutTime = 0.1f;  // 对于限定播放次数的动画, 会自动淡出

		// 3. 添加位移
		rb.AddForce (new Vector2 (0, accel));

		// 4. 将 Jump 同组的其他动画停掉, 并设置成 null
		// 将idel 状态停掉, 并设置成null
		if (_idelState != null) {
			_idelState.FadeOut (0.1f);
			_idelState = null;
		}

		// 将run 状态停掉, 并设置成null
		if (_runState != null) {
			_runState.FadeOut (0.1f);
			_runState = null;
		}
	}

}

```

完整项目地址: <a href="https://github.com/tackor/Unity-DragonBones-Demo.git">https://github.com/tackor/Unity-DragonBones-Demo.git</a>