import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
  styleUrls: ['./forget-password.component.scss']
})
export class ForgetPasswordComponent {

  public emailForm: FormGroup;

  public submitted: boolean = false;

  /**
   *
   */
  constructor(private fb: FormBuilder, 
    private message: NzMessageService,
    private authService: AuthenticationService) {
    this.emailForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    })
  }

  submit(): void{
    if (this.emailForm.invalid) {
      Object.values(this.emailForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error('Error when resetting password! \nPlease correct your input.')
      return;
    }

    
    this.submitted = true;
    this.authService.forgetPassword(this.email?.value).subscribe({
      next: () => {
        this.submitted = true;
      },
      error: (error) => {
      }
    })


  }

  get email(): AbstractControl<any,any> | null{
    return this.emailForm.get('email');
  }

}
