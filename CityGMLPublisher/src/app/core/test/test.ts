import { Scene, Vector2, Shape, ShapeBufferGeometry, Mesh, Vector3, Matrix4, BoxGeometry, PlaneGeometry, Object3D, Line, Path, GeometryUtils, Geometry, ShapeUtils, Face3, MeshBasicMaterial } from "three";
import { HomeComponent } from "../../home/home.component";
import { MaterialManager } from "../material.manager";

export class Test{
    scene: Scene;
    mesh: Object3D;
    vectorRotate: Vector3 = new Vector3(0,1,0);
    constructor(homeComponent: HomeComponent){
        this.scene = homeComponent.scene;
    }

    public draw(){
        let vertices: Vector2[] = [];
        vertices.push(new Vector2(1,1));
        vertices.push(new Vector2(6,1));
        vertices.push(new Vector2(6,6));
        vertices.push(new Vector2(1,6));
        vertices.push(new Vector2(1,1));

        let holesVertices: Vector2[] = [
            new Vector2(1,1),
            new Vector2(2,1),
            new Vector2(2,2),
            new Vector2(1,2),
           new Vector2(1,1),
        ];

        let _line = new Geometry();
        for(let v of holesVertices){
            _line.vertices.push(new Vector3(v.x, v.y, 0));
        }

        let _linemesh = new Line(_line, MaterialManager.LINE_MATERIAL);
        this.scene.add(_linemesh);

        this.drawShape(vertices, holesVertices);

        //GeometryUtils.merge();
        
        //let shape = new Shape(vertices);
        //shape.holes = [new Path(holesVertices)];
        //let shapeGeo = new ShapeBufferGeometry(shape);
        //shapeGeo.computeBoundingBox();
        //let centerX = (shapeGeo.boundingBox.min.x + shapeGeo.boundingBox.max.x)/2;
        //let centerY = (shapeGeo.boundingBox.min.y + shapeGeo.boundingBox.max.y)/2;
        //let centerZ = (shapeGeo.boundingBox.min.z + shapeGeo.boundingBox.max.z)/2;

        //let minX = shapeGeo.boundingBox.min.x;
        //let minY = shapeGeo.boundingBox.min.y;

        

        //centerY = centerY - centerY/2;

        //console.log(shapeGeo);

        //let center = new Vector3(centerX, centerY, centerZ);

        //shapeGeo.translate(-centerX, -centerY, 0);
        // let shapeGeo2 = new ShapeBufferGeometry(shape);
        // let mesh2 = new Mesh(shapeGeo2, MaterialManager.FLOOR_MATERIAL);
        // this.scene.add(mesh2);
        
        //shapeGeo.translate(-minX, -minY, 0);
        //this.mesh = new Mesh(shapeGeo, MaterialManager.FLOOR_MATERIAL);
        //this.mesh.rotateOnAxis(this.vectorRotate, -Math.PI/2);
        //this.mesh.position.set(minX, minY, 0);
        //this.mesh.translateX(minX);
        //this.mesh.translateY(minY);

        //.set(minX, minY, 0);


        //shapeGeo.center();
        //let planeGeo = new PlaneGeometry(1,1);
        //planeGeo.translate(0,1.5,0);
        

        //this.mesh = new Mesh(planeGeo, MaterialManager.FLOOR_MATERIAL);
        //this.mesh.position.setX(1);

        //let boxGeo = new BoxGeometry(1,1);
        //this.mesh = new Mesh(boxGeo, MaterialManager.FLOOR_MATERIAL);
        //let vR = new Vector3(1,1,0).normalize();
        

        //this.mesh.rotateOnWorldAxis(vR, Math.PI/2);
        
        // this.mesh.position.set(centerX, centerY, 0);
        
        // let m = new Matrix4();
        // m.makeRotationAxis(vR, Math.PI/2);

        // mesh.applyMatrix(m);

        //this.scene.add(this.mesh);


        //let line = new Line();
    }
    drawShape(vertices: Vector2[], holeVertices: Vector2[]){
        //var holes = [new Path()];
        var triangles, mesh;
        var geometry = new Geometry();
        var material = new MeshBasicMaterial();

        geometry.vertices = vertices.map(p => new Vector3(p.x, p.y, 0));

        triangles = ShapeUtils.triangulateShape( vertices, holeVertices );

        for( var i = 0; i < triangles.length; i++ ){
            geometry.faces.push( new Face3( triangles[i][0], triangles[i][1], triangles[i][2] ));
        }

        mesh = new Mesh( geometry, material );
        this.scene.add(mesh);
    }
    public animate(){
        //let vR = new Vector3(1,1,0).normalize();
        let v1 = new Vector3(0.7,1.3,0);
        let v2 = new Vector3(1,1,0);
        let v3 = v1.sub(v2);
        //this.mesh.rotateOnAxis(v3, new Date().getMilliseconds()/10000);
    }
}