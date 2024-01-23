import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmationDialogComponent } from './confirmation-dialog.component';

describe('ModalComponent', () => {
  let component: ConfirmationDialogComponent;
  let fixture: ComponentFixture<ConfirmationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ConfirmationDialogComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ConfirmationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should call this.activeModal.close(false) when component.decline is called', ()=> {
    const modalCloseSpy = spyOn(component.activeModal, 'close');

    component.decline(); 

    expect(modalCloseSpy).toHaveBeenCalledWith(false); 
  })

  it('should call this.activeModal.close(true) when component.decline is called', ()=> {
    const modalAcceptSpy = spyOn(component.activeModal, 'close');

    component.accept(); 

    expect(modalAcceptSpy).toHaveBeenCalledWith(true); 
  })

  it('should call this.activeModal.dismiss() when component.decline is called', ()=> {
    const modalDismissSpy = spyOn(component.activeModal, 'dismiss');

    component.dismiss(); 

    expect(modalDismissSpy).toHaveBeenCalledWith(true); 
  })

});
