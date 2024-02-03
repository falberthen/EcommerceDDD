import { Component, Input, inject } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl: 'confirmation-dialog.component.html'
})
export class ConfirmationDialogComponent {
  @Input({required: true}) title!: string;
  @Input({required: true}) message!: string;
  @Input({required: true}) btnOkText!: string;
  @Input({required: true}) btnCancelText!: string;
 
  activeModal = inject(NgbActiveModal);

  decline(): void {
    this.activeModal.close(false);
  }

  accept(): void {
    this.activeModal.close(true);
  }

  dismiss(): void {
    this.activeModal.dismiss();
  }
}
