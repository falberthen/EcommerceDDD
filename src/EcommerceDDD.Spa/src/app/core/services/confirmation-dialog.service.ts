import { Injectable, inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';

@Injectable()
export class ConfirmationDialogService {
  private modalService = inject(NgbModal);

  public confirm(
    title: string,
    message: string,
    btnOkText: string = 'OK',
    btnCancelText: string = 'Cancel',
    dialogSize: 'sm' | 'lg' = 'sm'
  ): Promise<boolean> {
    const modalRef = this.modalService.open(ConfirmationDialogComponent, {
      size: dialogSize,
    });
    
    const instance = modalRef.componentInstance;
    instance.title = title;
    instance.message = message;
    instance.btnOkText = btnOkText;
    instance.btnCancelText = btnCancelText;

    return modalRef.result;
  }
}
