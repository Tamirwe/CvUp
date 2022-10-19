import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";

export const LayoutNotAuth: React.FC = () => {
  return (
    <>
      <Outlet />
    </>
  );
};
