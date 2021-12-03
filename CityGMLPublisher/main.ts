import { app, BrowserWindow, screen, Menu, MenuItem, shell, dialog, SaveDialogReturnValue, ipcMain, MessageBoxOptions  } from 'electron';
import * as path from 'path';
import * as url from 'url';
const net = require('net');


let win, serve;
const args = process.argv.slice(1);
serve = args.some(val => val === '--serve');

var socketClient;

var currentJsonFile = "";

function createWindow() {

  const electronScreen = screen;
  const size = electronScreen.getPrimaryDisplay().workAreaSize;

  // Create the browser window.
  win = new BrowserWindow({
    x: 0,
    y: 0,
    width: size.width,
    height: size.height,
    webPreferences: {
      nodeIntegration: true,
    },
  });

  if (serve) {
    require('electron-reload')(__dirname, {
      electron: require(`${__dirname}/node_modules/electron`)
    });
    win.loadURL('http://localhost:4200');
  } else {
    win.loadURL(url.format({
      pathname: path.join(__dirname, 'dist/index.html'),
      protocol: 'file:',
      slashes: true
    }));
  }

  if (serve) {
    win.webContents.openDevTools();
  }

  // Emitted when the window is closed.
  win.on('closed', () => {
    // Dereference the window object, usually you would store window
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    win = null;
  });

  //Menu
  var menu = Menu.buildFromTemplate([
    {
      label: 'File',
      submenu: [
        {          
          label: 'Export',
          submenu: [{
            id: "miExportCityGML",
            label: 'CityGML',
            enabled: false,
            click() { callExternal(currentJsonFile); }
          }]
        },
        {
          id: "miClose",
          label: 'Close',
          enabled: false,
          click() { closeModel(); }
        },
        { type: 'separator' },
        { label: 'Exit', role: 'quit' }        
      ]
    }
  ])
  Menu.setApplicationMenu(menu);


  ipcMain.on('export-city-gml', (event, arg) => {
    callExternal(arg);
    //event.reply('asynchronous-reply', 'pong')
  })
  ipcMain.on('json-file-selected', (event, arg) => {
    currentJsonFile = arg;
    
    var menu = Menu.getApplicationMenu();
    var menuItem = menu.getMenuItemById("miExportCityGML");
    menuItem.enabled = true;
    menuItem = menu.getMenuItemById("miClose");
    menuItem.enabled = true;
  })

  ipcMain.on('3d-is-ready', () => {
    //console.log('3D is READY');
    connectSocket();
  });

  process.on('uncaughtException', function (err) {
      console.log(err);
  });
}

function connectSocket() {
    try {
        socketClient = net.connect({ host: '127.0.0.1', port: 11000 }, () => {
            // 'connect' listener
            console.log('connected to server!');
            //client.write('world!\r\n');
        });
        socketClient.on('data', (data) => {
            console.log('on_data ' + data.toString());
            //client.end();
            openModel(data.toString());

        });
        socketClient.on('end', () => {
            console.log('disconnected from server');
        });
    } catch (e) {
        console.log(e);
    }
}

function openModel(filePath) {
    //console.log('open_model ' + filePath);
    win.webContents.send('open-model', filePath);
}

function closeModel() {
  //ipcMain.emit("close-model");
  win.webContents.send('close-model');
  var menu = Menu.getApplicationMenu();
  var menuItem = menu.getMenuItemById("miExportCityGML");
  menuItem.enabled = false;
  menuItem = menu.getMenuItemById("miClose");
  menuItem.enabled = false;
}
function callExternal(jsonPath? : string) {
  showSaveDialog().then((result) => {
    if (result.canceled) {
      return;
    }

    //var executablePath = path.resolve('..\\CityGMLExporter\\bin\\Debug\\CityGMLExporter.exe');
    var executablePath = process.env.PORTABLE_EXECUTABLE_DIR + '\\CityGMLExporter\\CityGMLExporter.exe';

    //var execFile = require('child_process').execFile;
    //execFile(executablePath, console.log);
    
    var parameters = [jsonPath, result.filePath];
    
    const { spawn } = require('child_process');
    const ls = spawn(executablePath, parameters);

    ls.stdout.on('data', (data) => {
      console.log(`stdout: ${data}`);
    });

    ls.stderr.on('data', (data) => {
      console.error(`stderr: ${data}`);
    });

    ls.on('close', (code) => {
      console.log(`child process exited with code ${code}`);
      showInfoMessage(`Export completed.`);
    });
  });
  
}
function showSaveDialog(): Promise<SaveDialogReturnValue>{
  let options = {
    //Placeholder 1
    title: "Save file - Electron example",

    //Placeholder 2
    //defaultPath: "C:\\BrainBell.png",

    //Placeholder 4
    buttonLabel: "Save CityGML",

    //Placeholder 3
    filters: [
      { name: 'CityGML', extensions: ['gml'] }
    ]
  }
  return dialog.showSaveDialog(win, options);
}
function showInfoMessage(msg: string, title?: string): void {
  if (!title) {
    title = "Information";
  }
  let options: MessageBoxOptions = {
    type: 'info',
    title: title,
    message: msg
  }
  dialog.showMessageBox(win, options);
}


try {

  // This method will be called when Electron has finished
  // initialization and is ready to create browser windows.
  // Some APIs can only be used after this event occurs.
  app.on('ready', createWindow);

  // Quit when all windows are closed.
  app.on('window-all-closed', () => {
    // On OS X it is common for applications and their menu bar
    // to stay active until the user quits explicitly with Cmd + Q
    if (process.platform !== 'darwin') {
      app.quit();
    }
  });

  app.on('activate', () => {
    // On OS X it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if (win === null) {
      createWindow();
    }
  });

} catch (e) {
  // Catch Error
  // throw e;
}

