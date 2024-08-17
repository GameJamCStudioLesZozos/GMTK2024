using Godot;
using System;

public partial class CameraAutoScale : Camera2D
{
	[Export]
	public float CameraBaseZoom { get; set; } = 1f;

	private Vector2 CameraBaseZoomAsVec => new(CameraBaseZoom, CameraBaseZoom);

	public void HandleNewScaling(float newScaling)
	{
		Zoom = CameraBaseZoomAsVec / newScaling;
	}
}
