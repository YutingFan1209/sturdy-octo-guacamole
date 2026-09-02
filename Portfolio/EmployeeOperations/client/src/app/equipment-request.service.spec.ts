import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { EquipmentRequestService } from './equipment-request.service';
describe('EquipmentRequestService', () => {
  let service: EquipmentRequestService;
  let http: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(EquipmentRequestService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());
  it('sends expected version with a transition', () => {
    service.transition('id', 'approve', 4, 'ok').subscribe();
    const request = http.expectOne('/api/equipment-requests/id/approve');
    expect(request.request.body).toEqual({ expectedVersion: 4, reason: 'ok' });
    request.flush({});
  });
});
