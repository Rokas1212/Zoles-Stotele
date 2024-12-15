import React, { useEffect, useRef } from "react";
import * as atlas from "azure-maps-control";

const MapComponent: React.FC<{ addresses: string[] }> = ({ addresses }) => {
  const mapRef = useRef<atlas.Map | null>(null);
  const mapDivRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (!mapDivRef.current) return;

    const azureMapsKey = import.meta.env.VITE_AZURE_MAPS_API_KEY;

    if (!azureMapsKey) {
      console.error("Azure Maps API key is missing. Please check your environment variables.");
      return;
    }

    // Initialize Azure Maps
    mapRef.current = new atlas.Map(mapDivRef.current, {
      center: [23.8813, 55.1694],
      zoom: 6,
      authOptions: {
        authType: atlas.AuthenticationType.subscriptionKey,
        subscriptionKey: azureMapsKey,
      },
      disableTelemetry: true,
    });

    mapRef.current.events.add("ready", () => {
      const datasource = new atlas.source.DataSource();
      mapRef.current?.sources.add(datasource);

      const popup = new atlas.Popup({
        closeButton: true,
        closeOnMouseOut: true,
      });

      // Geocode addresses and add pins
      const geocodePromises = addresses.map(async (address) => {
        const response = await fetch(
          `https://atlas.microsoft.com/search/address/json?api-version=1.0&subscription-key=${azureMapsKey}&query=${encodeURIComponent(
            address
          )}`
        );
        const data = await response.json();
        const position = data.results[0]?.position;

        if (position) {
          const feature = new atlas.data.Feature(
            new atlas.data.Point([position.lon, position.lat]),
            { address } // Attach address as a property for popup
          );
          datasource.add(feature);
        }
      });

      Promise.all(geocodePromises).then(() => {
        // Add pins to the map
        const pinsLayer = new atlas.layer.SymbolLayer(datasource, undefined, {
          iconOptions: { image: "pin-round-blue" },
        });
        mapRef.current?.layers.add(pinsLayer);

        // Add hover behavior to show popups
        mapRef.current?.events.add("mouseover", pinsLayer, (e) => {
          if (e?.shapes && e.shapes[0] instanceof atlas.Shape) {
            const properties = e.shapes[0].getProperties() as { address?: string };
            const coordinates = e.shapes[0].getCoordinates();

            if (properties?.address) {
              popup.setOptions({
                content: `<div style="background: white; padding: 8px; border-radius: 4px; box-shadow: 0 2px 6px rgba(0,0,0,0.2);">
                  ${properties.address}
                </div>`,
                position: coordinates as atlas.data.Position,
              });
              popup.open(mapRef.current!);
            }
          }
        });

        // Hide the popup on mouse out
        mapRef.current?.events.add("mouseout", pinsLayer, () => {
          popup.close();
        });
      });
    });

    return () => {
      mapRef.current?.dispose();
    };
  }, [addresses]);

  return <div ref={mapDivRef} style={{ height: "400px", width: "100%", overflow: "auto" }} />;
};

export default MapComponent;