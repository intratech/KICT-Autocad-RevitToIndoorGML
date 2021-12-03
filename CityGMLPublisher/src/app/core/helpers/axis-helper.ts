import { HomeComponent } from "../../home/home.component";
import { Geometry, Vector3, Line } from "three";
import { MaterialManager } from "../material.manager";

export class AxisHelper {
    private scene;

    constructor(homeComponent: HomeComponent){
        this.scene = homeComponent.scene;
    }
    public draw(){
        let lineGeo = new Geometry();
        lineGeo.vertices.push(
            new Vector3(10,0,0),
            new Vector3(-10,0,0)
        );
        let line = new Line(lineGeo, MaterialManager.LINE_MATERIAL);
        this.scene.add(line);

        lineGeo = new Geometry();
        lineGeo.vertices.push(
            new Vector3(0,10,0),
            new Vector3(0,0,0),
            new Vector3(0,-10,0),
        );
        line = new Line(lineGeo, MaterialManager.LINE_MATERIAL2);
        this.scene.add(line);
    }
}