import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";

export const Layout: React.FC = () => {
  return (
    <>
      <Outlet />
    </>
  );
};
