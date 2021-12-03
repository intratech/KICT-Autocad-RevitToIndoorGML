import { Component, ElementRef, ViewChild, AfterViewInit, NgZone, OnDestroy, OnInit } from '@angular/core';
import * as THREE from 'three';
import { TrackballControls } from 'three/examples/jsm/controls/TrackballControls';
import { BufferGeometryUtils } from 'three/examples/jsm/utils/BufferGeometryUtils';
import { ElectronService } from '../core/services/electron/electron.service';
import { MainService } from '../core/services/http/main.service';
import { Model } from '../core/models/DTO';
import { WallManager } from '../core/wall.manager';
import { CeilingAndFloorManager } from '../core/ceiling-floor-manager';
//import { AxisHelper } from '../core/helpers/axis-helper';
import { Vector3, Vector2, Mesh } from 'three';
import { MaterialManager } from '../core/material.manager';
import { HierarchyManager } from '../core/hierarchy.manager';
import { TreeNode } from 'angular-tree-component';
import { Config } from '../config';
import { VisibilityManager } from '../core/visibility.manager';
import { MathUtil } from '../core/utils/math.util';
import { Guid } from '../core/utils/guid.util';
//import { Test } from '../core/test/test';

declare var $: any;
declare interface baseLine {
  startPoint: THREE.Vector3,
  endPoint: THREE.Vector3,
  distance: number,
  door,
  room
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  @ViewChild('threeDViewer', { static: true }) treeDViewer: ElementRef<HTMLCanvasElement>;
  @ViewChild('threeDViewerContainer', { static: true }) treeDViewerContainer: ElementRef<HTMLDivElement>;
  @ViewChild('fileInput', { static: true }) fileInput: ElementRef<HTMLInputElement>

  private frameId: number = null;
  public scene: THREE.Scene;
  private raycaster: THREE.Raycaster;
  private mouse: THREE.Vector2;
  private INTERSECTED: any = {};
    private geometry: any;
    public camera: THREE.PerspectiveCamera;
  private renderer: THREE.WebGLRenderer;
    public controls: TrackballControls;
  private material2: THREE.LineBasicMaterial;

  public hierarchy = [];
  public options = {};
  public selectedItemProperties = [];

  private wallManager: WallManager;
  private floorAndCeilingManager: CeilingAndFloorManager;
  public visibilityManager: VisibilityManager;
  //private test: Test;

  private jsonFilePath: string;
  public modelOpenning: boolean = false;

  public constructor(private ngZone: NgZone, private service: ElectronService, private httpService: MainService) {
    this.mouse = new THREE.Vector2();
    this.wallManager = new WallManager();
    this.floorAndCeilingManager = new CeilingAndFloorManager();
    this.visibilityManager = new VisibilityManager();
  }

  public ngOnInit() {

  }
  public ngAfterViewInit(): void {
    this.service.ipcRenderer.on('close-model', () => {      
      this.closeMode();
    });
    this.service.ipcRenderer.on('open-model', (evt, args) => {
      //console.log('on_client_open_model');
      //console.log(args);
      this.autoOpenModel(args);
    });

    this.treeDViewer.nativeElement.width = this.treeDViewerContainer.nativeElement.clientWidth;
    this.treeDViewer.nativeElement.height = this.treeDViewerContainer.nativeElement.clientHeight;
    this.init3D().then(() => {
      this.wallManager.init(this);
      this.floorAndCeilingManager.init(this);
      this.visibilityManager.init(this);

      //new AxisHelper(this).draw();
      //this.loadModel();
      //this.test = new Test(this);
      //this.test.draw();


        this.animate();

        this.service.ipcRenderer.send('3d-is-ready');
        //this.service.showErrorDialog("error123", 'Error');
    });
  }
  public ngOnDestroy() {
    if (this.frameId != null) {
      cancelAnimationFrame(this.frameId);
    }
  }

  private init3D(): Promise<void> {
    return new Promise((resolve, reject) => {
      //Scene
      this.scene = new THREE.Scene();
      this.scene.background = new THREE.Color(0x000000);

      window['scene'] = this.scene;

        //Camera
        this.camera = new THREE.PerspectiveCamera(75, this.treeDViewer.nativeElement.width / this.treeDViewer.nativeElement.height, 1, 1500000);
        this.camera.position.z = 15;
        window['camera'] = this.camera;

        //Renderer
        this.renderer = new THREE.WebGLRenderer({
          canvas: this.treeDViewer.nativeElement,
          alpha: true,    // transparent background
          antialias: true // smooth edges
        });

        this.renderer.setSize(this.treeDViewer.nativeElement.width, this.treeDViewer.nativeElement.height);
        this.renderer.gammaInput = true;
        this.renderer.gammaOutput = true;

        //Raycaster
        this.raycaster = new THREE.Raycaster();

        this.material2 = new THREE.LineBasicMaterial({ color: 0xc04829 });

        //Control
        this.controls = new TrackballControls(this.camera, this.renderer.domElement);

        //this.controls.addEventListener('change', this.render);
        this.controls.rotateSpeed = 4;
        this.controls.zoomSpeed = 2;
        this.controls.panSpeed = 3;
        this.controls.noZoom = false;
        this.controls.noPan = false;
        this.controls.staticMoving = true;
        this.controls.dynamicDampingFactor = 0.3;
        this.controls.keys = [65, 83, 68];
        //this.controls.addEventListener('change', this.renderCam);
        this.scene.add(this.camera);


        //Lights
        // var directionalLight = new THREE.DirectionalLight( 0xffffff, 0.6 );
        // directionalLight.position.set( 0.75, 0.75, 1.0 ).normalize();
        // scene.add( directionalLight );

        var ambientLight = new THREE.AmbientLight(0xcccccc, 0.2);
        this.scene.add(ambientLight);
        ambientLight.userData = { type: "LIGHT" };

        var hemiLight = new THREE.HemisphereLight(0xffffff, 0x444444);
        hemiLight.position.set(0, 20, 0);
        hemiLight.userData = { type: "LIGHT" };
        this.scene.add(hemiLight);

        var dirLight = new THREE.DirectionalLight(0xffffff, 0.5);
        dirLight.position.set(- 3, 10, - 10);
        dirLight.userData = { type: "LIGHT" };
        //dirLight.castShadow = true;
        //dirLight.shadow.camera.top = 2;
        //dirLight.shadow.camera.bottom = - 2;
        //dirLight.shadow.camera.left = - 2;
        //dirLight.shadow.camera.right = 2;
        //dirLight.shadow.camera.near = 0.1;
        //dirLight.shadow.camera.far = 40;
        this.scene.add(dirLight);

        this.addSky();

        //var lightHelper = new THREE.SpotLightHelper(dirLight);
        //this.scene.add(lightHelper);

        // const geometry = new THREE.BoxGeometry(1, 1, 1);
        // const material = new THREE.MeshBasicMaterial({ color: 0x00ff00 });
        // let cube = new THREE.Mesh( geometry, material );
        // this.scene.add(cube);

        resolve();
    });
  }
  private addSky() {
    var light = new THREE.DirectionalLight(0xaabbff, 0.3);
    light.position.x = 300;
    light.position.y = 250;
    light.position.z = - 500;

    light.userData = { type: "LIGHT" };
    this.scene.add(light);

    var vertexShader = document.getElementById('vertexShader').textContent;
    var fragmentShader = document.getElementById('fragmentShader').textContent;
    var uniforms = {
      topColor: { value: new THREE.Color(0x0077ff) },
      //bottomColor: { value: new THREE.Color(0xffffff) },
      bottomColor: { value: new THREE.Color(0xc4d0ff) },
      offset: { value: 400 },
      exponent: { value: 0.6 }
    };
    uniforms.topColor.value.copy(light.color);
    
    var skyMat = new THREE.ShaderMaterial({
      uniforms: uniforms,
      vertexShader: vertexShader,
      fragmentShader: fragmentShader,
      side: THREE.BackSide
    });

    //var skyGeo = new THREE.SphereBufferGeometry(4000, 32, 15);

    //var sky = new THREE.Mesh(skyGeo, skyMat);
    //sky.name = Config.SKY_MESH_NAME;

    //sky.userData = { type: "SKY" };
    //this.scene.add(sky);
  }
  private loadModel() {
    this.httpService.getObjects('assets/data/KICT.json').subscribe((res) => {
      this.addObjectsToScene(res as Model);
    });
  }
  private addObjectsToScene(json: Model) {
    this.floorAndCeilingManager.draw(json.rooms, json.levels, json.scale);
    this.wallManager.addRoom(json.rooms, json.doors, json.windows);

    //Set camera parameters
    let scalar = 1;

    //Get box of Model
    let box = json.bounding_box;
    if (!box) {
      let Obj3DArr = this.scene.children;//.filter(item => item.type == "Mesh" && item.name != "SKY");
      box = MathUtil.getBoxByObjArr(Obj3DArr);
  }


  if (box) {
      let center = new THREE.Vector3((box.max.x + box.min.x) / 2, (box.max.y + box.min.y) / 2, (box.max.z + box.min.z) / 2);
      let camDis = new THREE.Vector3().subVectors(box.max, box.min).length() * scalar;
      let camDir = new THREE.Vector3(0.5, 0.5, -0.707107);
      this.camera.position.x = center.x - camDir.x * camDis;
      this.camera.position.y = center.y - camDir.y * camDis;
      this.camera.position.z = center.z - camDir.z * camDis;
      this.camera.up.x = 0;
      this.camera.up.y = 0;
      this.camera.up.z = 1;
      //this.camera.lookAt(100000, 100000, 100000); -> no effect if use trackcontrol
      this.controls.target = center;
      this.render();
  }

    //Build hierarchy
    new HierarchyManager(this.wallManager, this.floorAndCeilingManager).loadHierarchy(json).then(_hierarchy => {
      this.hierarchy = _hierarchy;
    });
  }
  private closeMode() {
    this.scene.remove(...this.scene.children.filter(c => c.userData.type != 'LIGHT' && c.userData.type != 'SKY'));
    this.hierarchy = [];
    this.selectedItemProperties = [];

    this.wallManager = new WallManager();
    this.floorAndCeilingManager = new CeilingAndFloorManager();
    this.visibilityManager = new VisibilityManager();

    this.wallManager.init(this);
    this.floorAndCeilingManager.init(this);
    this.visibilityManager.init(this);
  }
  private autoOpenModel(path) {
    try {
        this.jsonFilePath = path;
        var jsonFile = this.service.getFile(this.jsonFilePath);
        this.service.ipcRenderer.send('json-file-selected', this.jsonFilePath);

        this.closeMode();

        this.addObjectsToScene(JSON.parse(jsonFile.toString()));
        this.modelOpenning = true;
        this.fileInput.nativeElement.value = "";
    }
    catch (err) {
        console.log(err.message + '\n' + err.stack);
        let error = err.message + '\n' + err.stack;
        this.service.showErrorDialog(error, 'Error');
        return;
    }
  }
  private animate(): void {
    // We have to run this outside angular zones,
    // because it could trigger heavy changeDetection cycles.
    this.ngZone.runOutsideAngular(() => {
      window.addEventListener('DOMContentLoaded', () => {
        this.render();

      });

      window.addEventListener('resize', () => {
        this.onWindowResize();
      });

      this.treeDViewerContainer.nativeElement.addEventListener('mousemove', (event) => {
        this.onDocumentMouseMove(event);
      });
      //document.addEventListener('click', (e) => {
      //  this.onDocumentClick(e);
      //});
      this.treeDViewer.nativeElement.addEventListener('click', (e) => {
        this.onDocumentClick(e);
      });
    });
  }
  private render() {
    this.frameId = requestAnimationFrame(() => {
      this.render();
    });

    //lightHelper.update();
    this.controls.update();

    //this.test.animate();

    // this.cube.rotation.x += 0.01;
      // this.cube.rotation.y += 0.01;
      this.renderer.render(this.scene, this.camera);
  }

  
  public openJsonFile() {
    this.fileInput.nativeElement.click();
  }

  public fileSelected(event) {
    try {
      this.jsonFilePath = this.fileInput.nativeElement.files[0].path;
      var jsonFile = this.service.getFile(this.jsonFilePath);
      this.service.ipcRenderer.send('json-file-selected', this.jsonFilePath);

      this.closeMode();

      this.addObjectsToScene(JSON.parse(jsonFile.toString()));
      this.modelOpenning = true;
      this.fileInput.nativeElement.value = "";
    }
    catch (err) {
      console.log(err.message + '\n' + err.stack);
      let error = err.message + '\n' + err.stack;
      this.service.showErrorDialog(error, 'Error');
      return;
    }
  }

  public onExportCityGML() {
    this.service.ipcRenderer.send('export-city-gml', this.jsonFilePath);
  }

  private onWindowResize() {
    console.log('resize');
    const width = this.treeDViewerContainer.nativeElement.clientWidth;
    const height = this.treeDViewerContainer.nativeElement.clientHeight;

        //Camera paras
    //let aspect = width / height;
    //let frustumSize = 400;

    //  this.camera.left = - frustumSize * aspect / 2;
    //  this.camera.right = frustumSize * aspect / 2;
    //  this.camera.top = frustumSize / 2;
    //  this.camera.bottom = - frustumSize / 2;
      this.camera.updateProjectionMatrix();

      this.renderer.setSize(width, height);

      //this.controls.handleResize();
  }

  private onDocumentMouseMove(event) {
    event.preventDefault();
    this.mouse.x = (event.clientX / this.treeDViewer.nativeElement.width) * 2 - 1;
    this.mouse.y = - (event.clientY / this.treeDViewer.nativeElement.height) * 2 + 1;
  }

  public onDocumentClick(event) {
    if (event.node) {
      //properties
      //console.log(event.node.data);
      if (event.node.data) {
        this.selectedItemProperties = event.node.data.properties;

        //Hide all meshes
        this.visibilityManager.showAllMeshes(false);
        console.log(event.node.data);

        //Show selected node
        this.visibilityManager.visibleNode(true, event.node.data);
        console.log(event.node.data);

        //Zoom to box
        let MeshesArr = new Array<Mesh>();
        this.visibilityManager.getMeshesFromSelectedNode(event.node.data, MeshesArr);
        let box = this.visibilityManager.getBoundingBox(MeshesArr);
        this.visibilityManager.zoomToBox(box);
      }
    }
    }
  //  public renderCam() {
  //  this.renderer.render(this.scene, this.camera);
  //}
}
