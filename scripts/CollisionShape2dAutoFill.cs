using Godot;
using System;

public partial class CollisionShape2dAutoFill : CollisionPolygon2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		/*
		Node node = GetParent();
		while (node != null && node is not Polygon2D)
			node = GetParent();
		if (node is null)
			return;
		Polygon = (node as Polygon2D).Polygon;
		*/
		Polygon = GetParent().GetParent<Polygon2D>().Polygon;
	}
}
