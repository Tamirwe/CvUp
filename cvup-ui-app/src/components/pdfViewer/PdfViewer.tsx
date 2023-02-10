import { observer } from "mobx-react";
import { LoadError, SpecialZoomLevel, Viewer } from "@react-pdf-viewer/core";
// import { defaultLayoutPlugin } from "@react-pdf-viewer/default-layout";

import { RenderGoToPageProps } from "@react-pdf-viewer/page-navigation";
import { toolbarPlugin, ToolbarSlot } from "@react-pdf-viewer/toolbar";
import {
  RenderCurrentScaleProps,
  RenderZoomInProps,
  RenderZoomOutProps,
} from "@react-pdf-viewer/zoom";

// import { toolbarPlugin } from "@react-pdf-viewer/toolbar";
// import type {
//   ToolbarSlot,
//   TransformToolbarSlot,
// } from "@react-pdf-viewer/toolbar";

import "@react-pdf-viewer/core/lib/styles/index.css";
import "@react-pdf-viewer/default-layout/lib/styles/index.css";

import { useStore } from "../../Hooks/useStore";

export const PdfViewer = observer(() => {
  const { candsStore } = useStore();
  // const defaultLayoutPluginInstance = defaultLayoutPlugin({
  //   sidebarTabs: (defaultTabs) => [],
  // });

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
  const toolbarPluginInstance = toolbarPlugin();
  const { Toolbar } = toolbarPluginInstance;

  const renderError = (error: LoadError) => {
    let message = "";
    switch (error.name) {
      case "InvalidPDFException":
        message = "The document is invalid or corrupted";
        break;
      case "MissingPDFException":
        message = "The document is missing";
        break;
      case "UnexpectedResponseException":
        message = "Unexpected server response";
        break;
      default:
        message = "Cannot load the document";
        break;
    }

    return (
      <div
        style={{
          alignItems: "center",
          border: "1px solid rgba(0, 0, 0, 0.3)",
          display: "flex",
          height: "100%",
          justifyContent: "center",
        }}
      >
        <div
          style={{
            backgroundColor: "#e53e3e",
            borderRadius: "0.25rem",
            color: "#fff",
            padding: "0.5rem",
          }}
        >
          {message}
        </div>
      </div>
    );
  };

  return (
    <div
      className="rpv-core__viewer"
      style={{
        // border: "1px solid rgba(0, 0, 0, 0.3)",
        border: "0",
        display: "flex",
        position: "relative",
        // height: "calc(100vh - 67px)",
        // marginTop: "4rem",
      }}
    >
      <div
        style={{
          alignItems: "center",
          backgroundColor: "#e1e1e1",
          border: "1px solid rgba(0, 0, 0, 0.2)",
          borderRadius: "2px",
          bottom: "5px",
          display: "flex",
          left: "50%",
          padding: "4px",
          position: "fixed",
          transform: "translate(-50%, 0)",
          zIndex: 2,
        }}
      >
        <Toolbar>
          {(props: ToolbarSlot) => {
            const {
              CurrentPageInput,
              Download,
              EnterFullScreen,
              GoToNextPage,
              GoToPreviousPage,
              NumberOfPages,
              Print,
              ZoomIn,
              ZoomOut,
            } = props;
            return (
              <>
                <div style={{ padding: "0px 2px" }}>
                  <ZoomOut />
                </div>
                <div style={{ padding: "0px 2px" }}>
                  <ZoomIn />
                </div>
                <div style={{ padding: "0px 2px", marginLeft: "auto" }}>
                  <GoToPreviousPage />
                </div>
                <div style={{ padding: "0px 2px", width: "4rem" }}>
                  <CurrentPageInput />
                </div>
                <div style={{ padding: "0px 2px" }}>
                  / <NumberOfPages />
                </div>
                <div style={{ padding: "0px 2px" }}>
                  <GoToNextPage />
                </div>
                <div style={{ padding: "0px 2px", marginLeft: "auto" }}>
                  <EnterFullScreen />
                </div>
                <div style={{ padding: "0px 2px" }}>
                  <Download />
                </div>
                <div style={{ padding: "0px 2px" }}>
                  <Print />
                </div>
              </>
            );
          }}
        </Toolbar>
      </div>
      <div
        style={{
          flex: 1,
          overflow: "hidden",
        }}
      >
        {candsStore.pdfUrl && (
          <Viewer
            defaultScale={SpecialZoomLevel.PageWidth}
            fileUrl={candsStore.pdfUrl}
            plugins={[toolbarPluginInstance]}
            renderError={renderError}
          />
        )}
      </div>
    </div>
  );
});
