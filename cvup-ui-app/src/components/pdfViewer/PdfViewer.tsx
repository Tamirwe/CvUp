import { observer } from "mobx-react";
import { Viewer } from "@react-pdf-viewer/core";
import { defaultLayoutPlugin } from "@react-pdf-viewer/default-layout";

// import { toolbarPlugin } from "@react-pdf-viewer/toolbar";
// import type {
//   ToolbarSlot,
//   TransformToolbarSlot,
// } from "@react-pdf-viewer/toolbar";

import "@react-pdf-viewer/core/lib/styles/index.css";
import "@react-pdf-viewer/default-layout/lib/styles/index.css";

import { useStore } from "../../Hooks/useStore";

export const PdfViewer = observer(() => {
  const { cvsStore } = useStore();
  const defaultLayoutPluginInstance = defaultLayoutPlugin({
    sidebarTabs: (defaultTabs) => [],
  });

  // const toolbarPluginInstance = toolbarPlugin();
  // const { renderDefaultToolbar, Toolbar } = toolbarPluginInstance;

  // const transform: TransformToolbarSlot = (slot: ToolbarSlot) => ({
  //   ...slot,
  //   Open: () => <></>,
  //   // Download: () => <></>,
  //   // DownloadMenuItem: () => <></>,
  //   // EnterFullScreen: () => <></>,
  //   // EnterFullScreenMenuItem: () => <></>,
  //   // SwitchTheme: () => <></>,
  //   // SwitchThemeMenuItem: () => <></>,
  // });

  return (
    <div style={{ height: "100vh", width: "100%" }}>
      {/* {cvsStore.cvId && ( */}

      <Viewer
        fileUrl={`https://localhost:7217/api/DD?id=${cvsStore.cvId}`}
        plugins={[defaultLayoutPluginInstance]}
      />
    </div>
  );
});
