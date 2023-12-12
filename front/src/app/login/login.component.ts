import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from 'src/services/auth.service';
import { CodeDialogComponent } from '../code-dialog/code-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { ChangePasswordComponent } from '../chang-password/chang-password.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  isVisible : boolean = false;

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required])
  })

  constructor(
    private dialog: MatDialog,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
  }

  login(){
    if (this.loginForm.valid) {
      const credentials = {
        email: this.loginForm.get('email')?.value,
        password: this.loginForm.get('password')?.value
      };

      this.authService.login(credentials).subscribe(
        (success) => {
          console.log(success)
          if (success) {
            if(success.isActivated)
            {
            this.router.navigate(['/home']);
            }
            else
            {
              const dialogRef = this.dialog.open(ChangePasswordComponent);
            }
          } else {
            this.snackBar.open('Invalid credentials', 'Close', { duration: 3000 });
          }
        },
        (error) => {
          console.error('Login error:', error);
          this.snackBar.open(error.error, 'Close', { duration: 3000 });
        }
      );
    } else {
      this.snackBar.open('Please fill in all required fields', 'Close', { duration: 3000 });
    }
  }

  activate(){
    const dialogRef = this.dialog.open(CodeDialogComponent);
  }

}
