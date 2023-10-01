import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  public loginForm: FormGroup;

  /** 
   *
   */
  constructor(private authservice: AuthenticationService,
    private fb: FormBuilder,
    private message: NzMessageService,
    private router: Router) {
      this.loginForm = this.fb.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required]]
      })
    
  }

  removeRejectErrorFromControls(): void{
    Object.values(this.loginForm.controls).forEach(control => {
        if (control.hasError('reject')) {
          let { ['reject']: _ignored, ...errors } = control.errors as any;
          console.log(errors)
          control.setErrors(Object.keys(errors).length ? errors : null);
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }

    });
  }

  submit(): void{
    if (this.loginForm.invalid) {
      Object.values(this.loginForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error('Error when logging in! \nPlease correct your input.')
      return;
    }

    this.authservice.login(this.email?.value, this.password?.value).subscribe({
      next: () => {
        this.router.navigate(['/playlists']);
      },
      error: (error) => {
        Object.values(this.loginForm.controls).forEach(control => {
          control.setErrors({reject: true});
        });
        this.message.error(error.error.message);
      }
    })

  }

  
  get email(){
    return this.loginForm.get('email');
  }

  get password(){
    return this.loginForm.get('password');
  }
}
