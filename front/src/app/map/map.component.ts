import { MapService, LatLng } from './../../services/map.service';
import { Component, EventEmitter, Input, NgZone, OnInit, Output, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {

  @Output() latLngSelected = new EventEmitter<LatLng>();
  @Input() markerPosition = {} as LatLng;
  map : google.maps.Map = {} as google.maps.Map;
  selectedLocationMarker: google.maps.Marker = {} as google.maps.Marker;

  constructor(private mapService: MapService,
              private ngZone: NgZone) { }

  ngOnInit(): void {
    this.mapService.getLoader().load().then(() => {
      this.initMap();
      this.mapService.geocoder = new google.maps.Geocoder();

      this.mapService.getCityCoordinates().subscribe({
        next: (value) => {
          console.log(value)
          this.map.setCenter(value);
        },
        error: (err) => {
          console.log(err);
        },
      })
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['markerPosition'] && !changes['markerPosition'].firstChange) {
      this.selectedLocationMarker.setPosition(this.markerPosition);
      this.map.setCenter(this.markerPosition);
    }
  }

  initMap(): void {
    this.map = new google.maps.Map(
      document.getElementById("map") as HTMLElement,
      {
        center: { lat: 45.236141, lng: 19.8367209 },
        zoom: 14,
        mapTypeControlOptions: {
          mapTypeIds: [google.maps.MapTypeId.ROADMAP]
        },
        disableDefaultUI: true,
        scaleControl: true,
        zoomControl: true,
        mapTypeId: 'roadmap',
        fullscreenControl: true,
      }
    );

    this.selectedLocationMarker = new google.maps.Marker();
    this.selectedLocationMarker.setMap(this.map);

    this.map.addListener("click", (mapsMouseEvent: google.maps.MapMouseEvent) => {
      this.selectedLocationMarker.setPosition(mapsMouseEvent.latLng);
      this.latLngSelected.emit({
        lat: mapsMouseEvent.latLng?.lat()!,
        lng: mapsMouseEvent.latLng?.lng()!,
      });
    });
  }

}
