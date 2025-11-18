package bro.gress.todosocialcompose

import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {

            MaterialTheme {
                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background
                ) {
                    ToDoListPage()
                }
            }
        }
    }
}

@Composable
fun NavigationBar(
    modifier: Modifier = Modifier,
    containerColor: Color = NavigationBarDefaults.containerColor,
    contentColor: Color = MaterialTheme.colorScheme.contentColorFor(containerColor),
    tonalElevation: Dp = NavigationBarDefaults.Elevation,
    windowInsets: WindowInsets = NavigationBarDefaults.windowInsets,
    content: @Composable RowScope.() -> Unit
): Unit {
}

@Composable
fun ButtonGrid() {
    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(16.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center
    ) {
        // First row
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .weight(1f),
            horizontalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            Button(
                onClick = { /* Handle button 1 click */ },
                modifier = Modifier
                    .weight(1f)
                    .fillMaxHeight()
            ) {
                Text("Button 1")
            }

            Button(
                onClick = { /* Handle button 2 click */ },
                modifier = Modifier
                    .weight(1f)
                    .fillMaxHeight()
            ) {
                Text("Button 2")
            }
        }

        Spacer(modifier = Modifier.height(16.dp))

        // Second row
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .weight(1f),
            horizontalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            Button(
                onClick = { /* Handle button 3 click */ },
                modifier = Modifier
                    .weight(1f)
                    .fillMaxHeight()
            ) {
                Text("Button 3")
            }

            Button(
                onClick = { /* Handle button 4 click */ },
                modifier = Modifier
                    .weight(1f)
                    .fillMaxHeight()
            ) {
                Text("Button 4")
            }
        }
    }
}

@Composable
fun ButtonGridPreview() {
    MaterialTheme {
        ButtonGrid()
    }
}