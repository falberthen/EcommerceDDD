import { NgbActiveModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationDialogComponent } from './confirmation-dialog.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';

describe('ModalComponent', () => {
  let component: ConfirmationDialogComponent;
  let fixture: ComponentFixture<ConfirmationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmationDialogComponent, NgbModule], 
      providers: [NgbActiveModal]
    }).compileComponents();

    fixture = TestBed.createComponent(ConfirmationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
