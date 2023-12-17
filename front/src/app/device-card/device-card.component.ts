import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-device-card',
  templateUrl: './device-card.component.html',
  styleUrls: ['./device-card.component.css', '../property-card/property-card.component.css']
})
export class DeviceCardComponent implements OnInit {

  constructor() { }

  @Input() device: any = {};

  ngOnInit(): void {
    console.log(this.device);
  }

}
