import { TestBed } from '@angular/core/testing';

import { InfluxDBDataService } from './influx-dbdata.service';

describe('InfluxDBDataServiceService', () => {
  let service: InfluxDBDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(InfluxDBDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
